import { useEffect, useState } from 'react';
import Markdown from 'react-markdown';
import remarkBreaks from 'remark-breaks';
import '../styles/privacy.css';

export default function PrivacyEdit() {
  const [markdown, setMarkdown] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const handleChange = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
    setMarkdown(event.target.value);
  };

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
        if (response.status === 405) {
          window.location.href = "/404";
        } 
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

  useEffect(() => {
    const textarea = document.querySelector('.markdown-editor') as HTMLTextAreaElement;
    if (textarea) {
      textarea.style.height = 'auto';
      textarea.style.height = `${textarea.scrollHeight}px`;
    }
  }, [markdown]);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setSubmitting(true);

    try {
      const response = await fetch(
        `${import.meta.env.VITE_REACT_APP_API_URL}/privacy/updatetext`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ Type: 'privacy', Content: markdown }),
          credentials: 'include',
        }
      );

      if (response.ok) {
        alert('Privacy text updated successfully');
      } else {
        const errorText = await response.text();
        alert(`Failed to update: ${errorText}`);
      }
    } catch (err) {
      console.error('Error:', err);
      alert('An error occurred. Please try again later.');
    } finally {
      setSubmitting(false);
    }
  };
  
  
  

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="markdown-container">
      <textarea
        className="markdown-editor"
        value={markdown}
        onChange={handleChange}
        placeholder="Type your Markdown here..."
      />
      <div className="markdown">
        <Markdown remarkPlugins={[remarkBreaks]}>{markdown}</Markdown>
      </div>
      <button onClick={handleSubmit} disabled={submitting}>
        {submitting ? 'Saving...' : 'Save'}
      </button>
    </div>
  );
}
