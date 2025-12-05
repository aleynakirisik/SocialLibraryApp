import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Navbar from './Navbar';
import { useNavigate } from 'react-router-dom';
import './Arama.css';

const Arama = () => {
  const [sorgu, setSorgu] = useState('');
  const [sonuclar, setSonuclar] = useState([]);
  const [vitrin, setVitrin] = useState(null);
  
  const [turFiltre, setTurFiltre] = useState('');
  const [puanFiltre, setPuanFiltre] = useState(0);
  const [yilFiltre, setYilFiltre] = useState(''); 

  const navigate = useNavigate();
  const PORT = "44321"; 

  const currentYear = new Date().getFullYear();
  const yillar = [];
  for (let i = currentYear; i >= 1950; i--) {
      yillar.push(i);
  } 
  useEffect(() => {
    axios.get(`https://localhost:${PORT}/api/Arama/Vitrin`)
         .then(res => setVitrin(res.data))
         .catch(err => console.error(err));
  }, []);

  const veriCek = () => {
    // vitrini göster
    if (!sorgu && !turFiltre && !yilFiltre && puanFiltre === 0) {
        setSonuclar([]);
        return;
    }

    axios.get(`https://localhost:${PORT}/api/Arama/Ara`, {
        params: {
            q: sorgu,
            tur: turFiltre,
            yil: yilFiltre,
            minPuan: puanFiltre
        }
    })
    .then(res => setSonuclar(res.data))
    .catch(err => console.error(err));
  };

  useEffect(() => {
    const delay = setTimeout(() => {
        veriCek();
    }, 500); // yarım saniye bekle 
    return () => clearTimeout(delay);
  }, [sorgu, turFiltre, puanFiltre, yilFiltre]);

  const aramaModu = sonuclar.length > 0 || sorgu || turFiltre || yilFiltre || puanFiltre > 0;

  return (
    <div>
      <Navbar />
      <div className="arama-container">
        
        <input 
          type="text" 
          placeholder="Kitap veya film adı ile ara..." 
          className="arama-input"
          value={sorgu}
          onChange={(e) => setSorgu(e.target.value)}
        />

        <div className="filtre-paneli" style={{marginBottom:'20px', display:'flex', gap:'10px'}}>
            
            <select value={turFiltre} onChange={(e) => setTurFiltre(e.target.value)}>
                <option value="">Tüm Türler</option> 
                <option value="Film">Filmler</option>
                <option value="Kitap">Kitaplar</option>
            </select>

            <select value={puanFiltre} onChange={(e) => setPuanFiltre(e.target.value)}>
                <option value="0">Tüm Puanlar</option>
                <option value="7">7+ Puan</option>
                <option value="8">8+ Puan</option>
                <option value="9">9+ Puan</option>
            </select>

            <select value={yilFiltre} onChange={(e) => setYilFiltre(e.target.value)}>
                <option value="">Tüm Yıllar</option> 
                {yillar.map(y => <option key={y} value={y}>{y}</option>)}
            </select>
        </div>

        {aramaModu ? (
            <div className="sonuc-listesi">
                {sonuclar.length === 0 ? <p>Sonuç bulunamadı.</p> : 
                 sonuclar.map(item => (
                    <div key={item.id} className="sonuc-karti" onClick={() => navigate(`/detay/${item.id}`)}>
                        <img src={item.gorselUrl} alt={item.baslik} onError={(e)=>{e.target.src='https://via.placeholder.com/150'}}/>
                        <div className="sonuc-bilgi">
                            <h4>{item.baslik}</h4>
                            <span className="tur-etiketi">{item.tur} {item.yayinYili ? `(${item.yayinYili.substring(0,4)})` : ''}</span>
                        </div>
                    </div>
                ))}
            </div>
        ) : (
           
            <div className="vitrin-alani">
                {vitrin && (
                    <>
                        <VitrinSeridi 
                            baslik="🔥 Platformda En Popülerler" 
                            items={vitrin.enPopuler} 
                            navigate={navigate} 
                        />
                        
                        <VitrinSeridi 
                            baslik="🏆 Platformda En Yüksek Puanlılar" 
                            items={vitrin.enYuksekPuan} 
                            navigate={navigate} 
                        />
                    </>
                )}
            </div>
        )}
      </div>
    </div>
  );
};

const VitrinSeridi = ({ baslik, items, navigate }) => (
    <div className="vitrin-serit">
        <h3>{baslik}</h3>
        <div className="serit-icerik">
            {items.map(item => (
                <div key={item.id} className="vitrin-kart" onClick={() => navigate(`/detay/${item.id}`)}>
                    <img src={item.gorselUrl} alt={item.baslik} onError={(e)=>{e.target.src='https://via.placeholder.com/150'}}/>
                </div>
            ))}
        </div>
    </div>
);

export default Arama;