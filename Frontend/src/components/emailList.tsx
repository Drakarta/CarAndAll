type EmailListProps = {
    emails: string[];
};

export default function EmailList({ emails }: EmailListProps) {
    return (
        <div className="email-list">
            <h2>Current Emails</h2>
            <ul>
                {emails.map((email, index) => (
                    <li key={index}>{email}</li>
                ))}
            </ul>
        </div>
    );
}