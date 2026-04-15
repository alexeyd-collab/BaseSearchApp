import React, { useState } from 'react';
import './index.css';

function App() {
  const [keyword, setKeyword] = useState('');
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleSearch = async (e) => {
    e.preventDefault();
    if (!keyword) return;
    
    setLoading(true);
    setError(null);
    try {
      const response = await fetch('/api/RepoSearch', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(keyword),
      });
      
      if (response.ok) {
        const data = await response.json();
        setResults(data);
      } else {
        const errData = await response.json().catch(() => ({}));
        setError(errData.error || 'Something went wrong connecting to the server.');
        setResults([]);
      }
    } catch (err) {
      console.error('Search error:', err);
      setError('A network error occurred. Please check your connection and try again.');
      setResults([]);
    } finally {
        setLoading(false);
    }
  };

  const handleBookmark = async (repoName) => {
    try {
      await fetch(`/api/SessionManager?name=${encodeURIComponent(repoName)}`, {
        method: 'POST',
      });
      console.log('Saved to session:', repoName);
    } catch (error) {
      console.error('Bookmark error:', error);
    }
  };

  return (
    <div className="app-container">
      <header className="header">
        <h1>Welcome to SearchApp</h1>
        <p>Your gateway to exploring GitHub repositories</p>
      </header>
      
      <form onSubmit={handleSearch} className="search-form">
        <input 
          type="text" 
          value={keyword} 
          onChange={(e) => setKeyword(e.target.value)} 
          placeholder="Enter search keyword" 
          className="search-input"
        />
        <button type="submit" className="search-button" disabled={loading}>
          {loading ? 'Searching...' : 'Search'}
        </button>
      </form>

      {error && (
        <div className="error-message" style={{ color: 'red', textAlign: 'center', marginTop: '1rem', padding: '1rem', background: '#ffecec', borderRadius: '4px' }}>
          {error}
        </div>
      )}

      <main className="results-container">
        {results.length > 0 ? (
          <ul className="results-grid">
            {results.map((repo, idx) => (
              <li key={idx} className="repo-card">
                <div className="repo-header">
                  <img src={repo.owner.avatar_url} alt={repo.owner.login} className="repo-avatar" />
                  <div className="repo-info">
                    <h2><a href={repo.html_url} target="_blank" rel="noreferrer">{repo.name}</a></h2>
                    <span className="repo-owner">@{repo.owner.login}</span>
                  </div>
                </div>
                <button 
                  onClick={() => handleBookmark(repo.name)} 
                  className="bookmark-btn">
                  Bookmark
                </button>
              </li>
            ))}
          </ul>
        ) : (
          !loading && <div className="no-results">No results to display. Execute a search!</div>
        )}
      </main>
    </div>
  );
}

export default App;
