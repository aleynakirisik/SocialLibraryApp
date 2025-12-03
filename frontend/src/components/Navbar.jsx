import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './Navbar.css';

const Navbar = () => {
  const navigate = useNavigate();

  const cikisYap = () => {
    localStorage.removeItem('kullanici');
    navigate('/');
  };

  return (
    <nav className="navbar">
      <div className="nav-logo">📚 Sosyal Kütüphane</div>
      <div className="nav-links">
        <Link to="/akis">Ana Sayfa</Link>
        <Link to="/arama">🔍 Keşfet</Link>
        <Link to="/profil">👤 Profilim</Link>
        <button onClick={cikisYap} className="cikis-btn">Çıkış</button>
      </div>
    </nav>
  );
};

export default Navbar;