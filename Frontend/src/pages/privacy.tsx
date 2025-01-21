import { useEffect, useState } from 'react'
import Markdown from 'react-markdown'

import '../styles/privacy.css'
import remarkBreaks from 'remark-breaks'

export default function Privacy() {
    const [markdown, setMarkdown] = useState(``)
    useEffect(() => {}, [markdown])
  return (
    <div className="markdown-container">
      <div className='markdown'>
        <Markdown remarkPlugins={[remarkBreaks]}>{markdown}</Markdown>
      </div>
    </div>
  )
}
