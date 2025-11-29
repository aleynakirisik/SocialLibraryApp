import React from 'react';
import { useNavigate, Link } from 'react-router-dom';
import './Form.css';

const Kayit = () => {
  const navigate = useNavigate();

  const handleKayit = (e) => {
    e.preventDefault();
    alert("Kayıt başarılı! Giriş sayfasına yönlendiriliyorsunuz.");
    navigate('/');
  };

  return (
    <div className="form-container">
      <form className="auth-form" onSubmit={handleKayit}>
        <h2>Kayıt Ol</h2>
        <input type="text" placeholder="Kullanıcı Adı" />
        <input type="email" placeholder="E-posta" />
        <input type="password" placeholder="Şifre" />
        <input type="password" placeholder="Şifre Tekrar" />
        <button type="submit">Kayıt Ol</button>
        <p>Zaten hesabın var mı? <Link to="/">Giriş Yap</Link></p>
      </form>
    </div>
  );
};

export default Kayit;