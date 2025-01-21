import { useEffect, useState } from 'react'
import Markdown from 'react-markdown'

import '../styles/privacy.css'
import remarkBreaks from 'remark-breaks'

export default function Privacy() {
    const [markdown, setMarkdown] = useState(``)
    const [loading, setLoading] = useState<boolean>(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
      const fetchPrivacyText = async () => {
        try {
          const response = await fetch(`${import.meta.env.VITE_REACT_APP_API_URL}/account/getuserbyid`, {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
            },
            credentials: 'include',
          });
  
          if (response.status === 200) {
            const data = await response.json();
            setMarkdown(data.markdown);
            setError(null);
          } else {
            setError(`Failed to fetch user data. Status: ${response.status}`);
          }
        } catch (err) {
          setError("An error occurred while fetching user data.");
          console.error(err);
        } finally {
          setLoading(false);
        }
      };
      fetchPrivacyText();
    }, []);
    
  return (
    <div className="markdown-container">
      <div className='markdown'>
        <Markdown remarkPlugins={[remarkBreaks]}>{markdown}</Markdown>
      </div>
    </div>
  )
}
