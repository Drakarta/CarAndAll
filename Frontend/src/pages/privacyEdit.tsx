import { useEffect, useState } from 'react';
import Markdown from 'react-markdown';
import remarkBreaks from 'remark-breaks';
import '../styles/privacy.css';

export default function PrivacyEdit() {
  // State to store the privacy text
  const [markdown, setMarkdown] = useState('');
  // State to determine if the page is still loading
  const [loading, setLoading] = useState(true);
  // State to store any errors that occur during fetching
  const [error, setError] = useState<string | null>(null);
  // State to determine if the user is submitting the form
  const [submitting, setSubmitting] = useState(false);

  // Handle changes to the textarea
  const handleChange = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
    setMarkdown(event.target.value);
  };

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

  // Automatically resize the textarea
  useEffect(() => {
    const textarea = document.querySelector('.markdown-editor') as HTMLTextAreaElement;
    if (textarea) {
      textarea.style.height = 'auto';
      textarea.style.height = `${textarea.scrollHeight}px`;
    }
  }, [markdown]);
  
  // Handle form submission
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
  
  
  
  // If the page is still loading, display a loading message
  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  // If there was an error, display it
  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="markdown-container">\
      {/* Textarea to edit the privacy text */}
      <textarea
        className="markdown-editor"
        value={markdown}
        onChange={handleChange}
        placeholder="Type your Markdown here..."
      />
      {/* display the privacy text as markdown */}
      <div className="markdown">
        <Markdown remarkPlugins={[remarkBreaks]}>{markdown}</Markdown>
      </div>
      {/* Button to submit the form */}
      <button onClick={handleSubmit} disabled={submitting}>
        {submitting ? 'Saving...' : 'Save'}
      </button>
    </div>
  );
}
