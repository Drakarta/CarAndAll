import { useEffect, useState } from "react";

import "../styles/voertuigenOverview.css";
import "../styles/backofficeaccount.css"
import EditBOAccount from "../components/editBOAccount";
import DeleteBOAccount from "../components/deleteBOAccount";
import CreateBOAccount from "../components/createBOAccount";

interface BackofficeAccount {
    id: string;
    naam: string;
    email: string;
    adres: string;
    telefoonNummer: string;
}

export default function BackofficeAccounts() {
    const [backofficeAccounts, setBackofficeAccounts] = useState<BackofficeAccount[]>([]);
    const [edit, setEdit] = useState<boolean>(false);
    const [selectedAccount, setSelectedAccount] = useState<BackofficeAccount | null>(null);
    const [create, setCreate] = useState<boolean>(false);
    
    useEffect(() => {
        const fetchBackofficeAccounts = async () => {
            try {
                const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/account/getbackofficeaccounts`, {
                    method: "GET",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    credentials: 'include',
                });

                if (response.status === 405) {
                    window.location.href = "/404";
                } 

                if (response.status === 200) {
                    const data = await response.json();
                    setBackofficeAccounts(data.accounts);
                } else {
                    console.error(`Failed to fetch backoffice accounts. Status: ${response.status}`);
                }
            } catch (err) {
                console.error("An error occurred while fetching backoffice accounts.");
                console.error(err);
            }
        };
        fetchBackofficeAccounts();
    }, []);

    function editAccount(account: BackofficeAccount) {
        setEdit(true);
        setSelectedAccount(account);
    }
    return (
        <>
            {edit && selectedAccount ? <EditBOAccount account={selectedAccount} edit={() => setEdit(false)} /> : null}
            {create ? <CreateBOAccount create={() => setCreate(false)}/> : null}
            <div className="overviewSection">
                <div className="headerFilter">
                    <h1>Back office accounts</h1>
                    <button onClick={() => setCreate(true)}>New Account</button>
                </div>
                <br/>
                <hr></hr>
                <br/>
                <section>
                    <div>
                <table className="accounts-tabel">
                    <thead>
                    <tr>
                    <th>Name</th>
                    <th>Email</th>
                    <th>Address</th>
                    <th>Phone Number</th>
                    <th></th>
                    </tr>
                </thead>
                <tbody>
                {backofficeAccounts.map((account: BackofficeAccount, index) => (
                    <tr key={index}>
                        <td>{account.naam}</td>
                        <td>{account.email}</td>
                        <td>{account.adres}</td>
                        <td>{account.telefoonNummer}</td>
                        <td>
                            <button onClick={() => editAccount(account)}>Edit</button>
                            <DeleteBOAccount accountId={account.id} />
                        </td>
                    </tr>
                    ))}
                </tbody>
                </table>
                    </div>
                </section>
            </div>
            
        </>
    )
    }
