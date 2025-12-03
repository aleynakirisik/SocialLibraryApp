import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Navbar from './Navbar';
import './Arama.css';
import { useNavigate } from 'react-router-dom';

const Arama = () => {
  const [sorgu, setSorgu] = useState('');
  const [sonuclar, setSonuclar] = useState([]);
  const [hata, setHata] = useState('');
  
  // FİLTRELEME STATE'LERİ
  const [turFiltre, setTurFiltre] = useState('Hepsi');
  const [puanFiltre, setPuanFiltre] = useState(0);

  const navigate = useNavigate();

  // --- PORT NUMARANI KONTROL ET (LaunchSettings.json'daki numara) ---
  const PORT = "44321"; 
  // ------------------------------------------------------------------

  // ÖNCE FONKSİYONU TANIMLIYORUZ (Hata almamak için yeri burası olmalı)
  const veriCek = (arananKelime) => {
    setHata('');
    
    // Eğer arama boşsa FİLTRELEME API'sine git, doluysa ARAMA API'sine git
    if (!arananKelime) {
         axios.get(`https://localhost:${PORT}/api/Sosyal/Filtrele?tur=${turFiltre}&minPuan=${puanFiltre}`)
             .then(res => setSonuclar(res.data))
             .catch(err => console.error(err));
         return;
    }

    const endpoint = `arama?q=${arananKelime}`;

    axios.get(`https://localhost:${PORT}/api/${endpoint}`)
      .then(res => {
        console.log("Gelen Veri:", res.data);
        setSonuclar(res.data);
      })
      .catch(err => {
        console.error("Bağlantı Hatası:", err);
        setHata("Veri çekilemedi. Backend kapalı veya port yanlış olabilir.");
      });
  };

  // SONRA USEEFFECT'LERİ YAZIYORUZ
  
  // 1. Sayfa ilk açıldığında çalışır (Vitrin/Filtre)
  useEffect(() => {
    veriCek(''); 
  }, [turFiltre, puanFiltre]); // Filtre değişince de çalışsın

  // 2. Arama kutusuna yazıldıkça çalışır
  useEffect(() => {
    if (sorgu.length > 0 && sorgu.length < 3) return;

    const delayDebounceFn = setTimeout(() => {
      veriCek(sorgu);
    }, 500); 

    return () => clearTimeout(delayDebounceFn);
  }, [sorgu]);

  return (
    <div>
      <Navbar />
      <div className="arama-container">
        <input 
          type="text" 
          placeholder="Film veya kitap ara... (En az 3 harf)" 
          className="arama-input"
          value={sorgu}
          onChange={(e) => setSorgu(e.target.value)}
        />
        
        {/* FİLTRE PANELİ */}
        <div className="filtre-paneli" style={{display:'flex', gap:'10px', marginBottom:'20px'}}>
            <select value={turFiltre} onChange={(e) => setTurFiltre(e.target.value)} style={{padding:'10px'}}>
                <option value="Hepsi">Tüm Türler</option>
                <option value="Film">Sadece Filmler</option>
                <option value="Kitap">Sadece Kitaplar</option>
            </select>

            <select value={puanFiltre} onChange={(e) => setPuanFiltre(e.target.value)} style={{padding:'10px'}}>
                <option value="0">Tüm Puanlar</option>
                <option value="7">7+ Puan</option>
                <option value="8">8+ Puan</option>
                <option value="9">9+ Puan</option>
            </select>
        </div>
        
        {hata && <div style={{color: 'red', textAlign: 'center', marginBottom: '20px'}}>{hata}</div>}

        <div className="sonuc-listesi">
          {sonuclar.map(item => (
            <div 
              key={item.id} 
              className="sonuc-karti"
              onClick={() => navigate(`/detay/${item.id}`)}
              style={{cursor: 'pointer'}}
            >
              <img src={item.gorselUrl} alt={item.baslik} onError={(e) => {e.target.src='https://via.placeholder.com/150?text=Resim+Yok'}} />
              <div className="sonuc-bilgi">
                <h4>{item.baslik}</h4>
                <span className="tur-etiketi">{item.tur}</span>
                {item.puan > 0 && <div style={{fontSize:'0.8rem', color:'#f5c518', marginTop:'5px'}}>⭐ {item.puan?.toFixed(1)}</div>}
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default Arama;