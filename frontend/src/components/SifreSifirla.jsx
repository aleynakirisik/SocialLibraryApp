import React, { useState } from 'react';
import { useSearchParams, useNavigate } from 'react-router-dom';
import axios from 'axios';
import './Form.css';

const SifreSifirla = () => {
  const [searchParams] = useSearchParams();
  const email = searchParams.get("email"); 
  
  const [sifre, setSifre] = useState('');
  const [sifreTekrar, setSifreTekrar] = useState('');
  const [mesaj, setMesaj] = useState('');
  const [hata, setHata] = useState('');
  const navigate = useNavigate();
  const PORT = "44321"; 

  const guncelle = (e) => {
    e.preventDefault();
    setHata(''); setMesaj('');

    if (sifre !== sifreTekrar) {
        setHata("Şifreler uyuşmuyor.");
        return;
    }

    axios.post(`https://localhost:${PORT}/api/Auth/SifreSifirla`, { email, yeniSifre: sifre })
      .then(res => {
          alert(res.data);
          navigate('/'); 
      })
      .catch(err => setHata(err.response?.data || "Hata oluştu."));
  };

  if (!email) return <div className="hata-kutusu">Geçersiz bağlantı.</div>;

  return (
    <div className="form-container">
      <form className="auth-form" onSubmit={guncelle}>
        <h2>Yeni Şifre Belirle</h2>
        <p style={{fontSize:'0.9rem'}}>Hesap: <strong>{email}</strong></p>
        
        {hata && <div className="hata-kutusu">{hata}</div>}

        <input type="password" placeholder="Yeni Şifre" value={sifre} onChange={e => setSifre(e.target.value)} required />
        <input type="password" placeholder="Yeni Şifre Tekrar" value={sifreTekrar} onChange={e => setSifreTekrar(e.target.value)} required />
        
        <button type="submit">Şifreyi Güncelle</button>
      </form>
    </div>
  );
};

export default SifreSifirla;