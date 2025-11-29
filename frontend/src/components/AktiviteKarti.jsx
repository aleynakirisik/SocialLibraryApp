import React from 'react';
import './AktiviteKarti.css';

const AktiviteKarti = ({ aktivite }) => {
  if (!aktivite) return null; // Hata önleyici

  const tarih = new Date(aktivite.zaman).toLocaleDateString('tr-TR');

  return (
    <div className="aktivite-karti">
      <div className="kart-ust">
        <img src={aktivite.profilResmi} alt="avatar" className="avatar" />
        <div className="bilgi">
          <span className="kullanici-adi">{aktivite.kullaniciAdi}</span>
          <span className="eylem">{aktivite.aciklama}</span>
          <span className="tarih">{tarih}</span>
        </div>
      </div>

      <div className="kart-icerik">
        <img src={aktivite.icerikGorsel} alt="poster" className="poster" />
        <div className="detaylar">
          <h4>{aktivite.icerikBaslik}</h4>
          
          {aktivite.tur === 'PUANLAMA' && (
            <div className="puan-kutusu">⭐ {aktivite.puan}/10</div>
          )}

          {aktivite.tur === 'YORUM' && (
            <div className="yorum-kutusu"><p>"{aktivite.yorumMetni}"</p></div>
          )}
        </div>
      </div>
    </div>
  );
};

export default AktiviteKarti;