import React from 'react';
import { useNavigate } from 'react-router-dom';
import { zamanHesapla } from '../utils'; // Az önce oluşturduğumuz fonksiyon
import './AktiviteKarti.css';

const AktiviteKarti = ({ aktivite }) => {
  const navigate = useNavigate();
  if (!aktivite) return null;

  // PDF : "3 saat önce" formatı
  const zamanMetni = zamanHesapla(aktivite.zaman);

  // PDF : Yorumun sadece ilk 150-200 karakteri (Excerpt)
  const yorumOzeti = aktivite.yorumMetni && aktivite.yorumMetni.length > 180 
      ? aktivite.yorumMetni.substring(0, 180) + "..." 
      : aktivite.yorumMetni;

  return (
    <div className="aktivite-karti">
      
      {/* --- HEADER (PDF 2.1.2.1) --- */}
      <div className="kart-ust">
        {/* Avatar ve İsim */}
        <img 
            src={aktivite.profilResmi} 
            alt="avatar" 
            className="avatar" 
            onClick={() => navigate(`/profil/${aktivite.kullaniciId}`)}
            style={{cursor:'pointer'}}
        />
        <div className="bilgi">
          <span className="kullanici-adi" onClick={() => navigate(`/profil/${aktivite.kullaniciId}`)}>
              {aktivite.kullaniciAdi}
          </span>
          <span className="eylem"> {aktivite.aciklama}</span>
          <span className="tarih">{zamanMetni}</span>
        </div>
      </div>

      {/* --- BODY (PDF 2.1.2.2 - Görsel Gösterim) --- */}
      <div className="kart-icerik" onClick={() => navigate(`/detay/${aktivite.icerikId}`)}>
        
        {/* Sol Taraf: Poster (Her durumda var) */}
        <img src={aktivite.icerikGorsel} alt="poster" className="poster" />
        
        <div className="detaylar">
          <h4 className="icerik-baslik">{aktivite.icerikBaslik}</h4>
          
          {/* DURUM 1: PUANLAMA AKTİVİTESİ */}
          {aktivite.tur === 'PUAN' && (
            <div className="rating-gosterimi">
               <div className="yildizlar">★★★★★</div>
               <div className="puan-degeri">{aktivite.puan}/10</div> {/* */}
            </div>
          )}

          {/* DURUM 2: YORUMLAMA AKTİVİTESİ */}
          {aktivite.tur === 'YORUM' && (
            <div className="review-gosterimi">
              <p className="yorum-metni">"{yorumOzeti}"</p>
              {/* Daha fazlasını oku linki */}
              {aktivite.yorumMetni && aktivite.yorumMetni.length > 180 && (
                  <span className="read-more">...daha fazlasını oku</span> 
              )}
            </div>
          )}

          {/* Diğer aktiviteler (Listeye ekleme vb.) */}
          {(aktivite.tur === 'IZLENECEK' || aktivite.tur === 'OKUNACAK') && (
              <div className="liste-bilgi">Listesine ekledi 📌</div>
          )}
        </div>
      </div>

      {/* --- FOOTER / ETKİLEŞİM --- */}
      <div className="kart-alt">
        <button>👍 Beğen</button>
        <button>💬 Yorum Yap</button>
      </div>
    </div>
  );
};

export default AktiviteKarti;