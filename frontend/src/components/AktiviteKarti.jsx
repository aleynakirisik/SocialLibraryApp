import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { zamanHesapla } from '../utils';
import './AktiviteKarti.css';

const AktiviteKarti = ({ aktivite }) => {
  const navigate = useNavigate();
  const PORT = "44321";

  const [begenildi, setBegenildi] = useState(aktivite.begendiMi || aktivite.BegendiMi || false);
  const [begeniSayisi, setBegeniSayisi] = useState(aktivite.begeniSayisi || aktivite.BegeniSayisi || 0);

  const [yorumlarAcik, setYorumlarAcik] = useState(false);
  const [altYorumlar, setAltYorumlar] = useState([]);
  const [yeniAltYorum, setYeniAltYorum] = useState("");
  const [yorumYukleniyor, setYorumYukleniyor] = useState(false);

  if (!aktivite) return null;
  const zamanMetni = zamanHesapla(aktivite.zaman);

  const yorumMetni = aktivite.yorumMetni || "";
  const yorumUzun = yorumMetni.length > 150;
  const yorumOzeti = yorumUzun ? yorumMetni.substring(0, 150) + "..." : yorumMetni;

  const detayaGit = (e) => {
      e?.stopPropagation();
      navigate(`/detay/${aktivite.icerikId}`);
  };
  const kayitliKullanici = JSON.parse(localStorage.getItem('kullanici'));
  const benimId = kayitliKullanici ? kayitliKullanici.id : 1;

  const handleBegen = (e) => {
    e.stopPropagation(); 
    axios.post(`https://localhost:${PORT}/api/Sosyal/Begen`, { kullaniciId: benimId, aktiviteId: aktivite.id })
    .then(res => {
        const yeniDurum = res.data;
        setBegenildi(yeniDurum);
        setBegeniSayisi(prev => yeniDurum ? prev + 1 : prev - 1);
    });
  };

  const handleYorumAcKapa = (e) => {
      e.stopPropagation();
      setYorumlarAcik(!yorumlarAcik);

      if (!yorumlarAcik && altYorumlar.length === 0) {
          setYorumYukleniyor(true);
          axios.get(`https://localhost:${PORT}/api/Sosyal/AktiviteYorumlari/${aktivite.id}`)
               .then(res => {
                   setAltYorumlar(res.data);
                   setYorumYukleniyor(false);
               });
      }
  };

  const altYorumGonder = (e) => {
      e.preventDefault(); // sayfa yenilenmesin
      if (!yeniAltYorum.trim()) return;

      axios.post(`https://localhost:${PORT}/api/Sosyal/AktiviteYorumEkle`, {
          aktiviteId: aktivite.id,
          kullaniciId: benimId,
          yorum: yeniAltYorum
      })
      .then(res => {
          setAltYorumlar([...altYorumlar, res.data]);
          setYeniAltYorum("");
      })
      .catch(err => alert("Yorum gönderilemedi."));
  };

  const icerikYorumOzeti = aktivite.yorumMetni && aktivite.yorumMetni.length > 180 
      ? aktivite.yorumMetni.substring(0, 180) + "..." : aktivite.yorumMetni;

  return (
    <div className="aktivite-karti">
      
      <div className="kart-ust">
        <img src={aktivite.profilResmi} alt="avatar" className="avatar" onClick={() => navigate(`/profil/${aktivite.kullaniciId}`)}/>
        <div className="bilgi">
          <span className="kullanici-adi" onClick={() => navigate(`/profil/${aktivite.kullaniciId}`)}>{aktivite.kullaniciAdi}</span>
          <span className="eylem"> {aktivite.aciklama}</span>
          <span className="tarih">{zamanMetni}</span>
        </div>
      </div>

      <div className="kart-icerik" style={{cursor: 'default'}}>

        <img src={aktivite.icerikGorsel} alt="poster" className="poster" onError={(e)=>{e.target.src='https://via.placeholder.com/150'}} />
        
        <div className="detaylar">
          <h4 className="icerik-baslik">{aktivite.icerikBaslik}</h4>
          {aktivite.tur === 'PUAN' && (
            <div className="rating-gosterimi">
               <div className="puan-degeri" style={{background:'#f5c518', color:'black', padding:'5px 10px'}}>
                  {aktivite.puan}/10 
               </div>
            </div>
          )}

          {aktivite.tur === 'YORUM' && (
            <div className="review-gosterimi">
              <p className="yorum-metni">"{yorumOzeti}"</p>

              {yorumUzun && (
                  <span className="daha-fazla-oku" onClick={detayaGit}>
                      ...daha fazlasını oku
                  </span>
              )}
            </div>
          )}
        </div>
      </div>

      <div className="kart-alt">
        <button onClick={handleBegen} style={{color: begenildi ? '#1877f2' : '#65676b'}}>
            {begenildi ? `👍 Beğendin ` : `👍 Beğen `}
        </button>
        <button onClick={handleYorumAcKapa}>💬 Yorum Yap</button>
      </div>

      {yorumlarAcik && (
          <div className="alt-yorumlar-bolumu" onClick={(e) => e.stopPropagation()}>

              <div className="alt-yorum-listesi">
                  {yorumYukleniyor ? <div style={{padding:'10px', fontSize:'0.8rem'}}>Yükleniyor...</div> : 
                   altYorumlar.length === 0 ? <div style={{padding:'10px', fontSize:'0.8rem', color:'#888'}}>İlk yorumu sen yap!</div> :
                   altYorumlar.map(y => (
                      <div key={y.id} className="alt-yorum-item">
                          <img src={y.profilResmi} alt="avt" className="alt-yorum-avatar"/>
                          <div className="alt-yorum-balon">
                              <strong>{y.kullaniciAdi}</strong>
                              <span>{y.yorum}</span>
                          </div>
                      </div>
                   ))
                  }
              </div>

              <form className="alt-yorum-form" onSubmit={altYorumGonder}>
                  <img src={kayitliKullanici?.profilResmi || "https://via.placeholder.com/30"} alt="ben" className="alt-yorum-avatar"/>
                  <input 
                    type="text" 
                    placeholder="Yorum yaz..." 
                    value={yeniAltYorum}
                    onChange={(e) => setYeniAltYorum(e.target.value)}
                  />
                  <button type="submit" disabled={!yeniAltYorum}>Gönder</button>
              </form>
          </div>
      )}
    </div>
  );
};

export default AktiviteKarti;