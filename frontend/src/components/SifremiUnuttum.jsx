import React, { useState } from 'react';
import axios from 'axios';
import { Link } from 'react-router-dom';
import './Form.css';

const SifremiUnuttum = () => {
  const [email, setEmail] = useState('');
  const [mesaj, setMesaj] = useState('');
  const [hata, setHata] = useState('');
  const PORT = "44321";

  const gonder = (e) => {
    e.preventDefault();
    setMesaj(''); setHata('');

    axios.post(`https://localhost:${PORT}/api/Auth/SifremiUnuttum`, { email })
      .then(res => setMesaj(res.data)) // "Link gönderildi" mesajı
      .catch(err => setHata(err.response?.data || "Hata oluştu."));
  };

  return (
    <div className="form-container">
      <form className="auth-form" onSubmit={gonder}>
        <h2>Şifre Sıfırlama</h2>
        <p style={{fontSize:'0.9rem', color:'#666'}}>E-posta adresinizi girin, size sıfırlama linki gönderelim.</p>
        
        {mesaj && <div className="basari-kutusu" style={{color:'green', marginBottom:'10px'}}>{mesaj}</div>}
        {hata && <div className="hata-kutusu">{hata}</div>}

        <input type="email" placeholder="E-posta Adresiniz" value={email} onChange={e => setEmail(e.target.value)} required />
        <button type="submit">Sıfırlama Linki Gönder</button>
        
        <p><Link to="/">Giriş ekranına dön</Link></p>
      </form>
    </div>
  );
};
export default SifremiUnuttum;