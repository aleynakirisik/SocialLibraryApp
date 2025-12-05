import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import axios from 'axios';
import './Form.css';

const Giris = () => {
  const [email, setEmail] = useState('');
  const [sifre, setSifre] = useState('');
  const [hata, setHata] = useState('');
  const navigate = useNavigate();
  const PORT = "44321";

  const handleGiris = (e) => {
    e.preventDefault();
    setHata('');

    axios.post(`https://localhost:${PORT}/api/Auth/Giris`, { email, sifre })
      .then(res => {
        localStorage.setItem('kullanici', JSON.stringify(res.data));
        navigate('/akis');
      })
      .catch(err => {
        setHata(err.response?.data || "E-posta veya şifre hatalı.");
      });
  };

  return (
    <div className="form-container">
      <form className="auth-form" onSubmit={handleGiris}>

        <span className="brand-logo">📚</span>
        <h2>Sosyal Kütüphane</h2>

        {hata && <div className="hata-kutusu">⚠️ {hata}</div>}

        <input 
            type="email" 
            placeholder="E-posta Adresiniz" 
            value={email} 
            onChange={(e) => setEmail(e.target.value)} 
            required 
        />
        <input 
            type="password" 
            placeholder="Şifreniz" 
            value={sifre} 
            onChange={(e) => setSifre(e.target.value)} 
            required 
        />
        
        <button type="submit">Giriş Yap</button>
        
        <div className="auth-footer">
            <Link to="/sifremi-unuttum" style={{display:'block', marginBottom:'10px', color:'#777', fontWeight:'normal'}}>
                Şifremi Unuttum?
            </Link>
            Hesabın yok mu? <Link to="/kayit">Kayıt Ol</Link>
        </div>

      </form>
    </div>
  );
};

export default Giris;