import React, { useEffect, useState } from 'react';
import axios from 'axios';
import AktiviteKarti from './AktiviteKarti';
import Navbar from './Navbar';
// CSS dosyasını çağırmıyoruz, çünkü App.css zaten genel olarak çalışıyor.

const AkisSayfasi = () => {
  const [aktiviteler, setAktiviteler] = useState([]);

  useEffect(() => {
    // Backend'den veriyi çek (Port numarana dikkat et!)
    // Eğer portun 7123 ise aşağıyı değiştirme, farklıysa düzelt.
    axios.get('https://localhost:44321/api/akis') 
      .then(response => {
        setAktiviteler(response.data);
      })
      .catch(error => console.error("Veri çekilemedi:", error));
  }, []);

  return (
    <div>
      <Navbar />
      <div className="akis-container">
        <h2>Akış</h2>
        {aktiviteler.length === 0 ? (
            <p style={{textAlign: 'center'}}>Yükleniyor veya henüz aktivite yok...</p>
        ) : (
            aktiviteler.map(akt => (
              <AktiviteKarti key={akt.id} aktivite={akt} />
            ))
        )}
      </div>
    </div>
  );
};

export default AkisSayfasi;