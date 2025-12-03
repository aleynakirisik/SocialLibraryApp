import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';
import Navbar from './Navbar';
import './Detay.css';

const Detay = () => {
  const { id } = useParams();
  const [icerik, setIcerik] = useState(null);
  const [yorumlar, setYorumlar] = useState([]); // Yorum listesi
  const [yeniYorum, setYeniYorum] = useState(""); // Yorum kutusu
  const [puan, setPuan] = useState(0); // Puan inputu
  
  // PORT NUMARANI KONTROL ET
  const PORT = "44321"; 

  // Verileri Çek
  useEffect(() => {
    // 1. İçerik Detayı
    axios.get(`https://localhost:${PORT}/api/Icerik/${id}`)
      .then(res => setIcerik(res.data));

    // 2. Yorumları Getir
    getYorumlar();
  }, [id]);

  const getYorumlar = () => {
    axios.get(`https://localhost:${PORT}/api/Kutuphane/Yorumlar/${id}`)
      .then(res => setYorumlar(res.data))
      .catch(err => console.log(err));
  };

  const kayitliKullanici = JSON.parse(localStorage.getItem('kullanici'));
  const benimId = kayitliKullanici ? kayitliKullanici.id : 0;

  // İŞLEM FONKSİYONUNU GÜNCELLE
  const islemYap = (tur) => { // (Eski adıyla listeyeEkle olabilir)
    
    if (!benimId) {
        alert("Lütfen önce giriş yapın!");
        return;
    }

    const istek = {
      kullaniciId: benimId, // <--- ARTIK 1 DEĞİL, GERÇEK ID
      icerikId: icerik.id,
      tur: tur,
      puan: tur === 'PUAN' ? parseFloat(puan) : 0,
      yorum: tur === 'YORUM' ? yeniYorum : null
    };

    axios.post(`https://localhost:${PORT}/api/Kutuphane/Ekle`, istek)
      .then(() => {
        alert("İşlem Başarılı!");
        if (tur === 'YORUM') {
            setYeniYorum(""); // Kutuyu temizle
            getYorumlar(); // Listeyi yenile
        }
      })
      .catch(() => alert("Hata oluştu!"));
  };

  if (!icerik) return <div className="yukleniyor">Yükleniyor...</div>;

  return (
    <div>
      <Navbar />
      <div className="detay-container">
        {/* ÜST KISIM (Aynı kalıyor) */}
        <div className="detay-sol">
          <img src={icerik.gorselUrl} alt={icerik.baslik} className="detay-poster" />
          <div className="aksiyon-butonlari">
             {/* Puanlama Alanı */}
             <div className="puan-alani">
                <input type="number" min="1" max="10" value={puan} onChange={(e)=>setPuan(e.target.value)} placeholder="0" />
                <button className="btn-puan" onClick={() => islemYap('PUAN')}>⭐ Puanla</button>
             </div>
             
             {/* Liste Butonları */}
             <button className="btn-liste" onClick={() => islemYap('IZLENECEK')}>➕ Listeme Ekle</button>
             <button className="btn-fav" onClick={() => islemYap('IZLENDI')}>✅ Bitirdim</button>
          </div>
        </div>

        <div className="detay-sag">
          <h1 className="detay-baslik">{icerik.baslik}</h1>
          <p className="ozet">{icerik.aciklama}</p>

          {/* --- YENİ BÖLÜM: YORUMLAR --- [cite: 70] */}
          <div className="yorumlar-bolumu">
            <h3>Yorumlar ({yorumlar.length})</h3>
            
            {/* Yorum Yapma Kutusu [cite: 72] */}
            <div className="yorum-yap">
                <textarea 
                    placeholder="Bu içerik hakkında ne düşünüyorsun?" 
                    value={yeniYorum}
                    onChange={(e) => setYeniYorum(e.target.value)}
                />
                <button onClick={() => islemYap('YORUM')}>Gönder</button>
            </div>

            {/* Yorum Listesi [cite: 71] */}
            <div className="yorum-listesi">
                {yorumlar.map(y => (
                    <div key={y.id} className="yorum-item">
                        <img src={y.avatar} alt="user" />
                        <div>
                            <strong>{y.kullaniciAdi}</strong>
                            <p>{y.yorum}</p>
                            <span className="tarih">{new Date(y.tarih).toLocaleDateString()}</span>
                        </div>
                    </div>
                ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Detay;