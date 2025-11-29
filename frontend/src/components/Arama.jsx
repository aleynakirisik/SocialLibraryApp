import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Navbar from './Navbar';
import './Arama.css';

const Arama = () => {
  const [sorgu, setSorgu] = useState('');
  const [sonuclar, setSonuclar] = useState([]);

  // Sayfa açılınca veya 'sorgu' değişince çalışır
  useEffect(() => {
    // Backend portunu kontrol et! (Senin portun neyse onu yaz: 7123 vs.)
    axios.get(`https://localhost:7123/api/arama?q=${sorgu}`)
      .then(res => setSonuclar(res.data))
      .catch(err => console.error(err));
  }, [sorgu]);

  return (
    <div>
      <Navbar /> {/* Menüyü ekledik */}
      <div className="arama-container">
        <input 
          type="text" 
          placeholder="Film veya kitap ara..." 
          className="arama-input"
          value={sorgu}
          onChange={(e) => setSorgu(e.target.value)}
        />
        
        <div className="sonuc-listesi">
          {sonuclar.map(item => (
            <div key={item.id} className="sonuc-karti">
              <img src={item.gorselUrl} alt={item.baslik} />
              <div className="sonuc-bilgi">
                <h4>{item.baslik}</h4>
                <span className="tur-etiketi">{item.tur}</span>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default Arama;