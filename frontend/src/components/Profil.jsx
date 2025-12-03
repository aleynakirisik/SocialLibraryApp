import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useParams, useNavigate } from 'react-router-dom';
import Navbar from './Navbar';
import './Profil.css';

const Profil = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  
  const [veri, setVeri] = useState(null);
  const [takipEdiyor, setTakipEdiyor] = useState(false);
  const [aktifSekme, setAktifSekme] = useState('izlenecek');
  
  // DÜZENLEME MODU
  const [duzenlemeModu, setDuzenlemeModu] = useState(false);
  const [yeniBio, setYeniBio] = useState("");
  const [yeniAvatar, setYeniAvatar] = useState("");

  const PORT = "44321"; // Portunu kontrol et

  // --- DÜZELTME BURADA BAŞLIYOR ---
  
  // 1. Tarayıcı hafızasından giriş yapan kişiyi al
  const kayitliKullanici = JSON.parse(localStorage.getItem('kullanici'));

  // Eğer giriş yapılmamışsa giriş sayfasına at
  useEffect(() => {
    if (!kayitliKullanici) {
        navigate('/');
    }
  }, [kayitliKullanici, navigate]);

  // Giriş yapanın ID'sini al
  const benimId = kayitliKullanici ? kayitliKullanici.id : 0;
  
  // Eğer URL'de ID varsa ona bak (/profil/5), yoksa bana bak (/profil)
  const profilId = id ? parseInt(id) : benimId; 

  // --------------------------------

  useEffect(() => {
    if (!kayitliKullanici) return;

    verileriGetir();
    
    // Başkasının profiline bakıyorsam takip durumunu sor
    if (profilId !== benimId) {
        axios.get(`https://localhost:${PORT}/api/Sosyal/Durum?ben=${benimId}&o=${profilId}`)
             .then(res => setTakipEdiyor(res.data));
    }
  }, [profilId]);

  const verileriGetir = () => {
    axios.get(`https://localhost:${PORT}/api/Kutuphane/Profil/${profilId}`)
      .then(res => {
          setVeri(res.data);
          setYeniBio(res.data.kullanici.biyografi || "");
          setYeniAvatar(res.data.kullanici.profilResmiUrl);
      })
      .catch(err => console.log("Profil yüklenirken hata:", err));
  }

  const takipIslemi = () => {
    axios.post(`https://localhost:${PORT}/api/Sosyal/TakipIslemi`, { benimId, baskasiId: profilId })
         .then(res => { setTakipEdiyor(!takipEdiyor); alert(res.data); });
  };

  const profiliKaydet = () => {
    const istek = { id: benimId, biyografi: yeniBio, profilResmiUrl: yeniAvatar };
    axios.put(`https://localhost:${PORT}/api/Kutuphane/ProfilGuncelle`, istek)
        .then(() => {
            setDuzenlemeModu(false);
            verileriGetir();
            // Navbar'daki resmi güncellemek için localStorage'ı da güncelle
            kayitliKullanici.profilResmi = yeniAvatar;
            localStorage.setItem('kullanici', JSON.stringify(kayitliKullanici));
            alert("Profil güncellendi!");
        });
  };

  if (!veri || !kayitliKullanici) return <div>Yükleniyor...</div>;

  return (
    <div>
      <Navbar />
      <div className="profil-container">
        <div className="profil-header">
            {duzenlemeModu ? (
                <div style={{display:'flex', flexDirection:'column', gap:'5px'}}>
                    <img src={yeniAvatar} alt="önizleme" className="profil-avatar" style={{opacity:0.5}}/>
                    <input type="text" value={yeniAvatar} onChange={e=>setYeniAvatar(e.target.value)} placeholder="Resim URL" style={{width:'100px'}}/>
                </div>
            ) : (
                <img src={veri.kullanici.profilResmiUrl} alt="avatar" className="profil-avatar"/>
            )}

            <div>
                <h1>{veri.kullanici.kullaniciAdi}</h1>
                
                {duzenlemeModu ? (
                    <textarea value={yeniBio} onChange={e=>setYeniBio(e.target.value)} rows="3" style={{width:'100%'}} />
                ) : (
                    <p>{veri.kullanici.biyografi || "Henüz biyografi yok."}</p>
                )}
                
                {profilId === benimId ? (
                    duzenlemeModu ? (
                        <div style={{marginTop:'10px'}}>
                            <button onClick={profiliKaydet} className="btn-kaydet">💾 Kaydet</button>
                            <button onClick={()=>setDuzenlemeModu(false)} className="btn-iptal">❌ İptal</button>
                        </div>
                    ) : (
                        <button onClick={()=>setDuzenlemeModu(true)} className="btn-duzenle">✏️ Profili Düzenle</button>
                    )
                ) : (
                    <button 
                        onClick={takipIslemi}
                        className={takipEdiyor ? "btn-takip-birak" : "btn-takip-et"}
                    >
                        {takipEdiyor ? 'Takipten Çık' : 'Takip Et'}
                    </button>
                )}
            </div>
        </div>

        {/* Son Aktiviteler */}
        <div className="son-aktiviteler-kutusu">
            <h3>📢 Son Hareketler</h3>
            <ul>
                {veri.sonAktiviteler?.length > 0 ? (
                    veri.sonAktiviteler.map(akt => (
                        <li key={akt.id}>
                            <span className="tarih">{new Date(akt.zaman).toLocaleDateString()}</span>
                            <strong>{akt.baslik}</strong> içeriği için: <em>{akt.aciklama}</em>
                        </li>
                    ))
                ) : (
                    <li>Henüz bir hareket yok.</li>
                )}
            </ul>
        </div>

        <div className="sekmeler">
            <button className={aktifSekme === 'izlenecek' ? 'aktif' : ''} onClick={() => setAktifSekme('izlenecek')}>🎬 İzlenecekler</button>
            <button className={aktifSekme === 'izlendi' ? 'aktif' : ''} onClick={() => setAktifSekme('izlendi')}>✅ İzlediklerim</button>
            <button className={aktifSekme === 'okunacak' ? 'aktif' : ''} onClick={() => setAktifSekme('okunacak')}>📚 Okunacaklar</button>
            <button className={aktifSekme === 'okundu' ? 'aktif' : ''} onClick={() => setAktifSekme('okundu')}>✔️ Okuduklarım</button>
        </div>

        <div className="liste-icerik">
            {aktifSekme === 'izlenecek' && <ListeGoster items={veri.izlenecekler} />}
            {aktifSekme === 'izlendi' && <ListeGoster items={veri.izlediklerim} />}
            {aktifSekme === 'okunacak' && <ListeGoster items={veri.okunacaklar} />}
            {aktifSekme === 'okundu' && <ListeGoster items={veri.okuduklarim} />}
        </div>
      </div>
    </div>
  );
};

const ListeGoster = ({ items }) => {
    if (!items || items.length === 0) return <p>Bu listede henüz içerik yok.</p>;
    return (
        <div className="grid-liste">
            {items.map(item => (
                <div key={item.id} className="mini-kart">
                    <img src={item.gorselUrl} alt={item.baslik} />
                    <span>{item.baslik}</span>
                </div>
            ))}
        </div>
    );
};

export default Profil;