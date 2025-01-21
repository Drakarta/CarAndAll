import { useEffect, useState } from 'react';
import Markdown from 'react-markdown';
import remarkBreaks from 'remark-breaks';
import '../styles/privacy.css';

export default function PrivacyEdit() {
  const [markdown, setMarkdown] = useState(``);

  function handleChange(event: React.ChangeEvent<HTMLTextAreaElement>) {
    setMarkdown(event.target.value);
  }

  useEffect(() => {
    const textarea = document.querySelector('.markdown-editor') as HTMLTextAreaElement;
    textarea.style.height = "auto";
    textarea.style.height = `${textarea.scrollHeight}px`;
  }, [markdown]);

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
    </div>
  );
}
