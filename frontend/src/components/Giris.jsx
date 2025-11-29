import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import './Form.css';

const Giris = () => {
  const [email, setEmail] = useState('');
  const [sifre, setSifre] = useState('');
  const navigate = useNavigate();

  const handleGiris = (e) => {
    e.preventDefault();
    if(email && sifre) {
        // Giriş başarılı varsayıyoruz
        localStorage.setItem('kullanici', 'GirisYapti');
        navigate('/akis'); 
    } else {
        alert("Lütfen tüm alanları doldurun!"); 
    }
  };

  return (
    <div className="form-container">
      <form className="auth-form" onSubmit={handleGiris}>
        <h2>Giriş Yap</h2>
        <input 
            type="email" 
            placeholder="E-posta" 
            value={email} 
            onChange={(e) => setEmail(e.target.value)} 
        />
        <input 
            type="password" 
            placeholder="Şifre" 
            value={sifre} 
            onChange={(e) => setSifre(e.target.value)} 
        />
        <button type="submit">Giriş Yap</button>
        <p>Hesabın yok mu? <Link to="/kayit">Kayıt Ol</Link></p>
      </form>
    </div>
  );
};

export default Giris;