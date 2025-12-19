import { Link } from 'react-router-dom';

export const NavBar = () => {
    return (
        <nav style={{ background: '#333', padding: '1rem', marginBottom: '2rem' }}>
            <div style={{ maxWidth: '800px', margin: '0 auto', display: 'flex', gap: '20px' }}>
                <Link to="/" style={{ color: 'white', textDecoration: 'none', fontWeight: 'bold' }}>
                    Recording
                </Link>
                <Link to="/analyze" style={{ color: 'white', textDecoration: 'none', fontWeight: 'bold' }}>
                    Analyze
                </Link>
            </div>
        </nav>
    );
};