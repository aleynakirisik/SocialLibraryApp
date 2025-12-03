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
        // Kullanıcıyı tarayıcıya kaydet
        localStorage.setItem('kullanici', JSON.stringify(res.data));
        navigate('/akis');
      })
      .catch(err => {
        //  "E-posta veya şifre hatalı" mesajını göster
        setHata(err.response?.data || "Giriş başarısız.");
      });
  };

  return (
    <div className="form-container">
      <form className="auth-form" onSubmit={handleGiris}>
        <h2>Giriş Yap</h2>
        {hata && <div className="hata-kutusu">{hata}</div>}
        
        {/* [cite: 20] İstenen Alanlar */}
        <input type="email" placeholder="E-posta" value={email} onChange={(e) => setEmail(e.target.value)} required />
        <input type="password" placeholder="Şifre" value={sifre} onChange={(e) => setSifre(e.target.value)} required />
        
        <button type="submit">Giriş Yap</button>
        
        {/*  Şifre Sıfırlama Linki */}
        <div style={{marginTop:'10px', fontSize:'0.9rem'}}>
            <Link to="/sifremi-unuttum">Şifremi Unuttum</Link> 
        </div>
        <p>Hesabın yok mu? <Link to="/kayit">Kayıt Ol</Link></p>
      </form>
    </div>
  );
};
export default Giris;