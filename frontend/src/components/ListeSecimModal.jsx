import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './ListeSecimModal.css';

const ListeSecimModal = ({ kullaniciId, icerikId, kapat }) => {
  const [listeler, setListeler] = useState([]);
  const [yeniListeAdi, setYeniListeAdi] = useState("");
  const PORT = "44321"; 

  useEffect(() => {
    fetchListeler();
  }, []);

  const fetchListeler = () => {
    axios.get(`https://localhost:${PORT}/api/OzelListe/Getir/${kullaniciId}`)
         .then(res => setListeler(res.data));
  };

  const listeOlustur = () => {
    if (!yeniListeAdi) return;
    axios.post(`https://localhost:${PORT}/api/OzelListe/Olustur`, { baslik: yeniListeAdi, kullaniciId })
         .then(() => {
             setYeniListeAdi("");
             fetchListeler(); 
         });
  };

  const listeyeEkle = (listeId) => {
    axios.post(`https://localhost:${PORT}/api/OzelListe/IcerikEkle`, { ozelListeId: listeId, icerikId })
         .then(() => {
             alert("Eklendi!");
             kapat();
         })
         .catch(err => alert(err.response?.data || "Hata"));
  };

  return (
    <div className="modal-overlay">
      <div className="modal-kutu">
        <div className="modal-header">
            <h3>Listeye Ekle</h3>
            <button onClick={kapat} className="kapat-btn">X</button>
        </div>

        <div className="yeni-liste-form">
            <input 
                type="text" 
                placeholder="Yeni Liste Adı..." 
                value={yeniListeAdi}
                onChange={e => setYeniListeAdi(e.target.value)}
            />
            <button onClick={listeOlustur}>Oluştur</button>
        </div>

        <div className="mevcut-listeler">
            {listeler.length === 0 ? <p>Hiç listen yok.</p> : listeler.map(liste => (
                <div key={liste.id} className="liste-satir" onClick={() => listeyeEkle(liste.id)}>
                    <span>📁 {liste.baslik}</span>
                    <span className="ekle-ikon">+</span>
                </div>
            ))}
        </div>
      </div>
    </div>
  );
};

export default ListeSecimModal;