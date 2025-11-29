import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Giris from './components/Giris';
import Kayit from './components/Kayit';
import AkisSayfasi from './components/AkisSayfasi';
import Arama from './components/Arama'; // Yeni ekledik
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
        </Routes>
      </div>
    </Router>
  );
}

export default App;