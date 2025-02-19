import { useEffect, useState } from 'react';
import Markdown from 'react-markdown';
import remarkBreaks from 'remark-breaks';

import '../styles/privacy.css';

export default function Privacy() {
  // State to store the privacy text
  const [markdown, setMarkdown] = useState<string>('');
  // State to determine if the page is still loading
  const [loading, setLoading] = useState<boolean>(true);
  // State to store any errors that occur during fetching
  const [error, setError] = useState<string | null>(null);

  // Fetch the privacy text from the API
  useEffect(() => {
    const fetchPrivacyText = async () => {
      try {
        const response = await fetch(
          `${import.meta.env.VITE_REACT_APP_API_URL}/privacy/gettext?type=privacy`,
          {
            method: 'GET',
            headers: {
              'Content-Type': 'application/json',
            },
            credentials: 'include',
          }
        );

        if (response.ok) {
          const content = await response.text(); // API returns plain text
          setMarkdown(content);
          setError(null);
        } else {
          setError(`Failed to fetch privacy text. Status: ${response.status}`);
        }
      } catch (err) {
        setError('An error occurred while fetching privacy text.');
        console.error(err);
      } finally {
        setLoading(false);
      }
    };

    fetchPrivacyText();
  }, []);

  // If the page is still loading, display a loading message
  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  // If there was an error, display it
  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    // Display the privacy text as markdown
    <div className="markdown-container">
      <div className="markdown">
        <Markdown remarkPlugins={[remarkBreaks]}>{markdown}</Markdown>
      </div>
    </div>
  );
}
