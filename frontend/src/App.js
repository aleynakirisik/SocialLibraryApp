import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Giris from './components/Giris';
import Kayit from './components/Kayit';
import AkisSayfasi from './components/AkisSayfasi';
import Arama from './components/Arama'; // Yeni ekledik
import Detay from './components/Detay';
import Profil from './components/Profil';
import SifremiUnuttum from './components/SifremiUnuttum';
import SifreSifirla from './components/SifreSifirla';
import './App.css';

function App() {
  return (
    <Router>
      <div className="App">
        <Routes>
          <Route path="/" element={<Giris />} />
          <Route path="/kayit" element={<Kayit />} />
          <Route path="/akis" element={<AkisSayfasi />} />
          <Route path="/arama" element={<Arama />} /> {/* Yeni rota */}
          <Route path="/detay/:id" element={<Detay />} />
          <Route path="/profil" element={<Profil />} />
          <Route path="/sifremi-unuttum" element={<SifremiUnuttum />} />
          <Route path="/sifre-sifirla" element={<SifreSifirla />} />
          <Route path="/profil" element={<Profil />} />      {/* Kendi profilim */}
          <Route path="/profil/:id" element={<Profil />} />  {/* Başkasının profili */}
        </Routes>
      </div>
    </Router>
  );
}

export default App;