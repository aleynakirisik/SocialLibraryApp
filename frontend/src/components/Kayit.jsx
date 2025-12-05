import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import axios from 'axios';
import './Form.css';

const Kayit = () => {
  const [formData, setFormData] = useState({
    kullaniciAdi: '',
    email: '',
    sifre: '',
    sifreTekrar: '' 
  });
  const [hata, setHata] = useState('');
  const navigate = useNavigate();
  const PORT = "44321";

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.type === 'text' && e.target.name !== 'kullaniciAdi' ? 'kullaniciAdi' : e.target.name]: e.target.value });

  };

  const handleKayit = (e) => {
    e.preventDefault();
    setHata('');

    if (formData.sifre !== formData.sifreTekrar) {
        setHata("Şifreler uyuşmuyor!");
        return;
    }

    axios.post(`https://localhost:${PORT}/api/Auth/Kayit`, {
        kullaniciAdi: formData.kullaniciAdi,
        email: formData.email,
        sifre: formData.sifre
    })
    .then(res => {
        alert(res.data);
        navigate('/'); 
    })
    .catch(err => {
        setHata(err.response?.data || "Kayıt başarısız.");
    });
  };

  return (
    <div className="form-container">
      <form className="auth-form" onSubmit={handleKayit}>
        
        <span className="brand-logo">✨</span>
        <h2>Aramıza Katıl</h2>
        <p className="auth-subtitle">Hemen ücretsiz hesabını oluştur</p>

        {hata && <div className="hata-kutusu">⚠️ {hata}</div>}
        
        <input name="kullaniciAdi" type="text" placeholder="Kullanıcı Adı" onChange={e => setFormData({...formData, kullaniciAdi: e.target.value})} required />
        <input name="email" type="email" placeholder="E-posta" onChange={e => setFormData({...formData, email: e.target.value})} required />
        <input name="sifre" type="password" placeholder="Şifre" onChange={e => setFormData({...formData, sifre: e.target.value})} required />
        <input name="sifreTekrar" type="password" placeholder="Şifre Tekrar" onChange={e => setFormData({...formData, sifreTekrar: e.target.value})} required />
        
        <button type="submit">Kayıt Ol</button>
        
        <div className="auth-footer">
            Zaten hesabın var mı? <Link to="/">Giriş Yap</Link>
        </div>
      </form>
    </div>
  );
};

export default Kayit;