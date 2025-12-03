import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import axios from 'axios';
import './Form.css';

const Kayit = () => {
  const [formData, setFormData] = useState({
    kullaniciAdi: '',
    email: '',
    sifre: '',
    sifreTekrar: '' // [cite: 19] Şifre tekrar alanı
  });
  const [hata, setHata] = useState('');
  const navigate = useNavigate();
  const PORT = "44321"; // Portunu kontrol et

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.type === 'text' && e.target.name !== 'kullaniciAdi' ? 'kullaniciAdi' : e.target.name]: e.target.value });
    // Input isimlerini aşağıda düzelttim, burayı basitleştirelim:
    // setFormData({...formData, [e.target.name]: e.target.value});
  };

  const handleKayit = (e) => {
    e.preventDefault();
    setHata('');

    // Validasyonlar
    if (formData.sifre !== formData.sifreTekrar) {
        setHata("Şifreler uyuşmuyor!");
        return;
    }

    // Backend'e İstek At
    axios.post(`https://localhost:${PORT}/api/Auth/Kayit`, {
        kullaniciAdi: formData.kullaniciAdi,
        email: formData.email,
        sifre: formData.sifre
    })
    .then(res => {
        alert(res.data);
        navigate('/'); // Girişe yönlendir
    })
    .catch(err => {
        //  Backend'den gelen net hata mesajını göster
        setHata(err.response?.data || "Kayıt başarısız.");
    });
  };

  return (
    <div className="form-container">
      <form className="auth-form" onSubmit={handleKayit}>
        <h2>Kayıt Ol</h2>
        {hata && <div className="hata-kutusu">{hata}</div>}
        
        {/* [cite: 19] İstenen Alanlar */}
        <input name="kullaniciAdi" type="text" placeholder="Kullanıcı Adı" onChange={e => setFormData({...formData, kullaniciAdi: e.target.value})} required />
        <input name="email" type="email" placeholder="E-posta" onChange={e => setFormData({...formData, email: e.target.value})} required />
        <input name="sifre" type="password" placeholder="Şifre" onChange={e => setFormData({...formData, sifre: e.target.value})} required />
        <input name="sifreTekrar" type="password" placeholder="Şifre Tekrar" onChange={e => setFormData({...formData, sifreTekrar: e.target.value})} required />
        
        <button type="submit">Kayıt Ol</button>
        <p>Zaten hesabın var mı? <Link to="/">Giriş Yap</Link></p>
      </form>
    </div>
  );
};
export default Kayit;