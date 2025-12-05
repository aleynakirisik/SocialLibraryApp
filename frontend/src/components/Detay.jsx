import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';
import Navbar from './Navbar';
import ListeSecimModal from './ListeSecimModal';
import './Detay.css';

const Detay = () => {
  const { id } = useParams();
  const [data, setData] = useState(null); 
  const [yorumlar, setYorumlar] = useState([]);
  const [yeniYorum, setYeniYorum] = useState("");
  const [puan, setPuan] = useState(0);
  const [hoverPuan, setHoverPuan] = useState(0);
  const [modalAcik, setModalAcik] = useState(false);
  
  const [duzenlenenYorumId, setDuzenlenenYorumId] = useState(null);
  const [duzenlemeMetni, setDuzenlemeMetni] = useState("");

  const PORT = "44321"; 
  const kayitliKullanici = JSON.parse(localStorage.getItem('kullanici'));
  const benimId = kayitliKullanici ? kayitliKullanici.id : 0;

  useEffect(() => {
    fetchDetay();
    getYorumlar();
  }, [id]);

  const fetchDetay = () => {
    axios.get(`https://localhost:${PORT}/api/Icerik/${id}?kullaniciId=${benimId}`)
      .then(res => {
          setData(res.data);
          if(res.data.kullaniciPuani) setPuan(parseFloat(res.data.kullaniciPuani));
      })
      .catch(err => console.error(err));
  };

  const getYorumlar = () => {
    axios.get(`https://localhost:${PORT}/api/Kutuphane/Yorumlar/${id}`)
      .then(res => setYorumlar(res.data))
      .catch(err => console.error(err));
  };

  const islemYap = (tur, yorumMetni = null, gelenPuan = null) => {
    if (!benimId) { alert("Lütfen giriş yapın!"); return; }

    const istek = {
      kullaniciId: benimId,
      icerikId: parseInt(id),
      tur: tur,
      puan: tur === 'PUAN' ? (gelenPuan !== null ? gelenPuan : puan) : 0,
      yorum: yorumMetni || (tur === 'YORUM' ? yeniYorum : null)
    };

    axios.post(`https://localhost:${PORT}/api/Kutuphane/Ekle`, istek)
      .then(() => {
        if (tur === 'YORUM') { setYeniYorum(""); getYorumlar(); }
        if (tur === 'PUAN') { alert(`Puan Kaydedildi: ${istek.puan}`); fetchDetay(); } 
        if (tur.includes('IZLE') || tur.includes('OKU')) { alert("Listeye Eklendi!"); }
      })
      .catch(() => alert("Hata oluştu!"));
  };

  const yorumSil = (yorumId) => {
      if(!window.confirm("Yorumu silmek istediğinize emin misiniz?")) return;
      axios.delete(`https://localhost:${PORT}/api/Kutuphane/YorumSil/${yorumId}?kullaniciId=${benimId}`)
           .then(() => { getYorumlar(); }) 
           .catch(err => alert("Silinemedi: " + err.response?.data));
  };

  const duzenlemeyiBaslat = (yorum) => {
      setDuzenlenenYorumId(yorum.id);
      setDuzenlemeMetni(yorum.yorum);
  };

  const yorumGuncelle = () => {
      axios.put(`https://localhost:${PORT}/api/Kutuphane/YorumGuncelle`, { yorumId: duzenlenenYorumId, yeniMetin: duzenlemeMetni })
           .then(() => { 
               setDuzenlenenYorumId(null); 
               getYorumlar(); 
           });
  };

  if (!data) return <div className="yukleniyor">Yükleniyor...</div>;
  const { icerik, platformPuani, toplamOy } = data;

  return (
    <div>
      <Navbar />
      <div className="detay-container">

        <div className="detay-sol">
          <img src={icerik.gorselUrl} alt={icerik.baslik} className="detay-poster" />
          
          <div className="aksiyon-butonlari">
             <div className="yildiz-kutusu">
                <div className="yildizlar">
                    {[...Array(10)].map((star, index) => {
                        const ratingValue = index + 1;
                        return (
                            <span 
                                key={index} 
                                className="yildiz-ikon"
                                style={{
                                    color: ratingValue <= (hoverPuan || puan) ? "#ffc107" : "#e4e5e9",
                                    cursor: "pointer", fontSize: "1.5rem"
                                }}
                                onClick={() => { 
                                    setPuan(ratingValue); 
                                    islemYap('PUAN', null, ratingValue); 
                                }} 
                                onMouseEnter={() => setHoverPuan(ratingValue)}
                                onMouseLeave={() => setHoverPuan(0)}
                            >★</span>
                        );
                    })}
                </div>
                <div style={{textAlign:'center', fontSize:'0.9rem'}}>Senin Puanın: {puan}</div>
             </div>
             
             <button className="btn-liste" onClick={() => islemYap(icerik.icerikTuru === 'Film' ? 'IZLENECEK' : 'OKUNACAK')}>
                 {icerik.icerikTuru === 'Film' ? '➕ İzlenecek' : '➕ Okunacak'}
             </button>
             <button className="btn-fav" onClick={() => islemYap(icerik.icerikTuru === 'Film' ? 'IZLENDI' : 'OKUNDU')}>
                 {icerik.icerikTuru === 'Film' ? '✅ İzledim' : '✅ Okudum'}
             </button>
             
             <button className="btn-ozel-liste" onClick={() => setModalAcik(true)}>
                 📑 Özel Listeye Ekle
             </button>
          </div>
        </div>

        <div className="detay-sag">
          <h1 className="detay-baslik">{icerik.baslik} <span className="yil">({icerik.yayinYili?.substring(0,4)})</span></h1>
          
          <div className="platform-puan-kutusu">
              <div className="puan-daire">{platformPuani}</div>
              <div className="puan-bilgi">
                  <span className="puan-etiket">Platform Puanı</span>
                  <span className="oy-sayisi">{toplamOy} kullanıcı oy verdi</span>
              </div>
          </div>

          <div className="kunye">
             <p><strong>Tür:</strong> {icerik.icerikTuru}</p>
             {icerik.icerikTuru === 'Film' ? 
                <p><strong>Yönetmen:</strong> {icerik.yonetmen}</p> : 
                <p><strong>Yazar:</strong> {icerik.yazarlar} • <strong>Sayfa:</strong> {icerik.sayfaSayisi}</p>
             }
          </div>

          <div className="ozet-kutusu">
            <h3>Özet</h3>
            <p>{icerik.aciklama}</p>
          </div>

          <div className="yorumlar-bolumu">
            <h3>Yorumlar ({yorumlar.length})</h3>
            
            <div className="yorum-yap">
                <textarea placeholder="Yorumun nedir?" value={yeniYorum} onChange={(e) => setYeniYorum(e.target.value)} />
                <button onClick={() => islemYap('YORUM')}>Gönder</button>
            </div>

            <div className="yorum-listesi">
                {yorumlar.map(y => (
                    <div key={y.id} className="yorum-item">
                        <img 
                            src={y.avatar} 
                            alt="user" 
                            onError={(e) => { e.target.src = 'https://via.placeholder.com/50?text=User'; }} // Resim yoksa bunu göster
                        />
                        <div style={{width:'100%'}}>
                            <div className="yorum-baslik">
                                <strong>{y.kullaniciAdi}</strong>
                                <span className="tarih">{new Date(y.tarih).toLocaleDateString()}</span>
                            </div>
                        
                            {duzenlenenYorumId === y.id ? (
                                <div className="duzenleme-alani">
                                    <textarea value={duzenlemeMetni} onChange={e=>setDuzenlemeMetni(e.target.value)} />
                                    <button onClick={yorumGuncelle} className="btn-kaydet-kucuk">Kaydet</button>
                                    <button onClick={()=>setDuzenlenenYorumId(null)} className="btn-iptal-kucuk">İptal</button>
                                </div>
                            ) : (
                                <p>{y.yorum}</p>
                            )}

                            {y.kullaniciAdi === kayitliKullanici.kullaniciAdi && !duzenlenenYorumId && (
                                <div className="yorum-aksiyonlari">
                                    <button onClick={()=>duzenlemeyiBaslat(y)}>Düzenle</button>
                                    <button onClick={()=>yorumSil(y.id)} style={{color:'red'}}>Sil</button>
                                </div>
                            )}
                        </div>
                    </div>
                ))}
            </div>
          </div>
        </div>
      </div>
      {modalAcik && (
          <ListeSecimModal 
              kullaniciId={benimId} 
              icerikId={icerik.id} 
              kapat={() => setModalAcik(false)} 
          />
      )}
    </div>
  );
};

export default Detay;