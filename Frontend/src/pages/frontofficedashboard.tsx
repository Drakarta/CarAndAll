import React, { useEffect, useState } from "react";

interface VerhuurAanvraag {
  AanvraagID: number;
  Status: string;
}

const VerhuurAanvragen: React.FC = () => {
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
      if (!response.ok) throw new Error("Kon verhuuraanvragen niet ophalen.");
      const data = await response.json();
      setAanvragen(data.map((item: any) => ({
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
          schadeInfo: schadeDetails ? schadeDetails : undefined, // Send only filled-in schadeInfo
        }),
      });
  
      if (!response.ok) {
        const errorData = await response.json();
        if (errorData.errors && errorData.errors.newStatus) {
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
    <div>
      <h1>Verhuur Aanvragen</h1>

      {error && <p style={{ color: "red" }}>{error}</p>}
      {loading && <p>Loading...</p>}

      <ul>
        {aanvragen.map((aanvraag, index) => (
          <li key={index}>
            <div>
              <span>ID: {aanvraag.AanvraagID}, Status: {aanvraag.Status}</span>
              <br />
              <label>
                Nieuwe Status:
                <select
                  value={aanvraagStatuses.get(aanvraag.AanvraagID) ?? aanvraag.Status}
                  onChange={(e) => setAanvraagStatuses(new Map(aanvraagStatuses).set(aanvraag.AanvraagID, e.target.value))}
                >
                    <option value="">Kies een status</option>
                  <option value="innemen">Innemen</option>
                  <option value="uitgeven">Uitgeven</option>
                  <option value="in reparatie">In reparatie</option>
                </select>
              </label>
              <br />
              <label>
                Schade:
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
              </label>
              {schadeChecked.get(aanvraag.AanvraagID) && (
                <div>
                  <label>
                    Schade Informatie:
                    <textarea
                      value={schadeInfo.get(aanvraag.AanvraagID) ?? ""}
                      onChange={(e) => setSchadeInfo(new Map(schadeInfo).set(aanvraag.AanvraagID, e.target.value))}
                    />
                  </label>
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

export default VerhuurAanvragen;
