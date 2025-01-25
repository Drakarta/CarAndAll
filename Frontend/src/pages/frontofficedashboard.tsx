import { useEffect, useState } from "react";
import "../styles/FrontOffice.css";

interface VerhuurAanvraag {
  AanvraagID: number;
  Status: string;
}

export default function VerhuurAanvragen() {
  const [aanvragen, setAanvragen] = useState<VerhuurAanvraag[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [aanvraagStatuses, setAanvraagStatuses] = useState<Map<number, string>>(new Map());
  const [schadeInfo, setSchadeInfo] = useState<Map<number, string | null>>(new Map());
  const [schadeChecked, setSchadeChecked] = useState<Map<number, boolean>>(new Map());

  useEffect(() => {
    fetchAanvragen();
  }, []);

  const fetchAanvragen = async () => {
    try {
      setLoading(true);
      const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/FrontOffice/GetVerhuurAanvragenWithStatus`, {
        credentials: "include",
      });
      if (response.status === 405) {
        window.location.href = "/404";
      }
      if (!response.ok) throw new Error("Kon verhuuraanvragen niet ophalen.");
      const data = await response.json();
      setAanvragen(data.map((item: { aanvraagID: number; status: string }) => ({
        AanvraagID: item.aanvraagID,
        Status: item.status,
      })));
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setLoading(false);
    }
  };

  const handleChangeStatus = async (aanvraagId: number) => {
    const newStatus = aanvraagStatuses.get(aanvraagId);
    const schade = schadeChecked.get(aanvraagId);
    const schadeDetails = schade ? schadeInfo.get(aanvraagId) : null;

    if (!newStatus) {
      alert("Vul een status in!");
      return;
    }

    // Debugging van de status en schade-informatie
    console.log("Status", newStatus);
    console.log("Schade details", schadeDetails);
    console.log("Schade", schade);

    try {
      setLoading(true);
      const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/FrontOffice/ChangeStatus`, {
        method: "POST",
        credentials: "include",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          aanvraagId,
          newStatus,
          schadeInfo: schadeDetails ?? undefined, // Send only filled-in schadeInfo
        }),
      });
      if (!response.ok) {
        const errorData = await response.json();
        if (errorData.errors?.newStatus) {
          alert(errorData.errors.newStatus.join(", "));
        } else {
          throw new Error("Fout bij bijwerken van status.");
        }
      } else {
        const success = await response.json();
        if (success) {
          alert("Status bijgewerkt!");
          fetchAanvragen(); // Lijst opnieuw laden
        } else {
          alert("Bijwerken van status mislukt.");
        }
      }
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="verhuur-aanvragen">
      <h1>Verhuur Aanvragen</h1>

      {error && <p style={{ color: "red" }}>{error}</p>}
      {loading && <p>Loading...</p>}

      <ul className="aanvragen">
        {aanvragen.map((aanvraag) => (
          <li key={aanvraag.AanvraagID} className="aanvraag-box">
            <div>
              <span>ID: {aanvraag.AanvraagID}, Status: {aanvraag.Status}</span>
              <br />
              <label>
                Nieuwe Status:
                <div>
              <select
                  value={aanvraagStatuses.get(aanvraag.AanvraagID) ?? aanvraag.Status}
                  onChange={(e) => setAanvraagStatuses(new Map(aanvraagStatuses).set(aanvraag.AanvraagID, e.target.value))}
                >
                  {aanvraag.Status === "uitgegeven" ? (
                    <>
                      <option value="">Selecteer een status</option>
                      <option value="ingenomen">Innemen</option>
                      <option value="in reparatie">In reparatie</option>
                    </>
                  ) : (
                    <>
                      <option value="">Selecteer een status</option>
                      <option value="uitgegeven">Uitgeven</option>
                    </>
                  )}
                </select>
                </div>
                </label>
              <br />
              <label>
                Schade:
                <div>
                  <input
                    type="checkbox"
                    checked={schadeChecked.get(aanvraag.AanvraagID) ?? false}
                    onChange={(e) => {
                      const checked = e.target.checked;
                      setSchadeChecked(new Map(schadeChecked).set(aanvraag.AanvraagID, checked));
                      if (!checked) {
                        setSchadeInfo(new Map(schadeInfo).set(aanvraag.AanvraagID, null));
                      }
                    }}
                  />
                </div>
              </label>
              {schadeChecked.get(aanvraag.AanvraagID) && (
                <div>
                  <label htmlFor={`schadeInfo-${aanvraag.AanvraagID}`}>
                    Schade Informatie:
                  </label>
                  <textarea
                    id={`schadeInfo-${aanvraag.AanvraagID}`}
                    value={schadeInfo.get(aanvraag.AanvraagID) ?? ""}
                    onChange={(e) => setSchadeInfo(new Map(schadeInfo).set(aanvraag.AanvraagID, e.target.value))}
                  />
                </div>
              )}
              <button onClick={() => handleChangeStatus(aanvraag.AanvraagID)}>Update Status</button>
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};