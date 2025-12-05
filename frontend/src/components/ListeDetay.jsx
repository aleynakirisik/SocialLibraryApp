import React, { useEffect, useState } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import axios from 'axios';
import Navbar from './Navbar';
import './ListeDetay.css'; 

const ListeDetay = () => {
  const { id } = useParams(); 
  const location = useLocation(); 
  const navigate = useNavigate();
  
  const [icerikler, setIcerikler] = useState([]);
  const [yukleniyor, setYukleniyor] = useState(true);
  
  const listeBasligi = location.state?.baslik || "Liste Detayı";
  const PORT = "44321"; 

  useEffect(() => {
    axios.get(`https://localhost:${PORT}/api/OzelListe/Detay/${id}`)
      .then(res => {
        setIcerikler(res.data);
        setYukleniyor(false);
      })
      .catch(err => {
        console.error(err);
        setYukleniyor(false);
      });
  }, [id]);

  return (
    <div>
      <Navbar />
      <div className="liste-detay-container">
        
        <div className="liste-header">
            <button onClick={() => navigate(-1)} className="geri-btn">← Geri</button>
            <h1>📁 {listeBasligi}</h1>
            <span className="sayac">{icerikler.length} İçerik</span>
        </div>

        {yukleniyor ? <p>Yükleniyor...</p> : (
            <div className="liste-grid">
                {icerikler.length === 0 ? (
                    <div className="bos-liste">
                        <p>Bu liste henüz boş.</p>
                        <button onClick={() => navigate('/arama')}>İçerik Ekle</button>
                    </div>
                ) : (
                    icerikler.map(item => (
                        <div key={item.id} className="liste-kart" onClick={() => navigate(`/detay/${item.id}`)}>
                            <img src={item.gorselUrl} alt={item.baslik} onError={(e)=>{e.target.src='https://via.placeholder.com/150'}} />
                            <div className="kart-bilgi">
                                <h4>{item.baslik}</h4>
                                <span className="tur-badge">{item.icerikTuru}</span>
                            </div>
                        </div>
                    ))
                )}
            </div>
        )}
      </div>
    </div>
  );
};

export default ListeDetay;