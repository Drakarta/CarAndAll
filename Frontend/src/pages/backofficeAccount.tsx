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
    // List of backoffice accounts gotten from the API
    const [backofficeAccounts, setBackofficeAccounts] = useState<BackofficeAccount[]>([]);
    // State to determine if the edit account modal should be shown
    const [edit, setEdit] = useState<boolean>(false);
    // State to determine which account should be edited
    const [selectedAccount, setSelectedAccount] = useState<BackofficeAccount | null>(null);
    // State to determine if the create account modal should be shown
    const [create, setCreate] = useState<boolean>(false);
    
    // Fetch backoffice accounts from the API
    // returns all accounts with the role "Backoffice Medewerker"
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

    // Function to set the edit state to true and set the selected account
    function editAccount(account: BackofficeAccount) {
        setSelectedAccount(account);
        setEdit(true);
    }
    return (
        <>
            {/* If the edit state and selectedAccount exist then show the EditBOAccount component */}
            {edit && selectedAccount ? <EditBOAccount account={selectedAccount} edit={() => setEdit(false)} /> : null}
            {/* If the create state is true then show the CreateBOAccount component */}
            {create ? <CreateBOAccount create={() => setCreate(false)}/> : null}
            {/* Overview section */}
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
                {/* Map through the backoffice accounts and show them in a table */}
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
