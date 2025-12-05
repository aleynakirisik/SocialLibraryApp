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
  
  const [duzenlemeModu, setDuzenlemeModu] = useState(false);
  const [yeniBio, setYeniBio] = useState("");
  const [yeniAvatar, setYeniAvatar] = useState("");
  const [ozelListeler, setOzelListeler] = useState([]);
  const [yeniListeAdi, setYeniListeAdi] = useState("");
const [listeOlusturmaModu, setListeOlusturmaModu] = useState(false);
  const PORT = "44321"; 

  const kayitliKullanici = JSON.parse(localStorage.getItem('kullanici'));

  useEffect(() => {
    if (!kayitliKullanici) {
        navigate('/');
    }
  }, [kayitliKullanici, navigate]);

  const benimId = kayitliKullanici ? kayitliKullanici.id : 0;
  
  const profilId = id ? parseInt(id) : benimId; 
  const ozelListeleriGetir = () => {
    axios.get(`https://localhost:${PORT}/api/OzelListe/Getir/${profilId}`)
         .then(res => setOzelListeler(res.data));
};
useEffect(() => {
    if (aktifSekme === 'ozelliste') ozelListeleriGetir();
}, [aktifSekme]);

  useEffect(() => {
    if (!kayitliKullanici) return;

    verileriGetir();
    
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
         .then(res => { 
             setTakipEdiyor(!takipEdiyor); 
         })
         .catch(err => console.error("Takip işlemi hatası:", err));
  };

  const profiliKaydet = () => {
    const istek = { id: benimId, biyografi: yeniBio, profilResmiUrl: yeniAvatar };
    axios.put(`https://localhost:${PORT}/api/Kutuphane/ProfilGuncelle`, istek)
        .then(() => {
            setDuzenlemeModu(false);
            verileriGetir();
            kayitliKullanici.profilResmi = yeniAvatar;
            localStorage.setItem('kullanici', JSON.stringify(kayitliKullanici));
            alert("Profil güncellendi!");
        });
  };
  
  const listeOlustur = () => {
    if (!yeniListeAdi.trim()) return;

    axios.post(`https://localhost:${PORT}/api/OzelListe/Olustur`, { 
        baslik: yeniListeAdi, 
        kullaniciId: benimId 
    })
    .then(() => {
        setYeniListeAdi(""); 
        setListeOlusturmaModu(false); 
        ozelListeleriGetir(); 
        alert("Liste oluşturuldu!");
    })
    .catch(err => alert("Hata oluştu."));
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

        <div className="son-aktiviteler-kutusu">
            <h3>📢 Son Hareketler</h3>
            <div className="aktivite-listesi-profil">
                {veri.sonAktiviteler?.length > 0 ? (
                    veri.sonAktiviteler.map(akt => (
                        <div 
                            key={akt.id} 
                            className="aktivite-satir"
                            
                        >
                            <div className={`aktivite-ikon ${akt.tur}`}>
                                {akt.tur === 'PUAN' && '⭐'}
                                {akt.tur === 'YORUM' && '💬'}
                                {akt.tur === 'IZLENDI' && '👁️'}
                                {akt.tur === 'OKUNDU' && '📖'}
                                {akt.tur === 'IZLENECEK' && '📌'}
                                {akt.tur === 'OKUNACAK' && '🔖'}
                            </div>

                            <div className="aktivite-metin">
                                <div className="aktivite-baslik-satiri">
                                    <strong>{akt.baslik}</strong>
                                    <span className="tarih">{new Date(akt.zaman).toLocaleDateString('tr-TR')}</span>
                                </div>
                                <p className="aktivite-aciklama">{akt.aciklama}</p>
                            </div>
                        </div>
                    ))
                ) : (
                    <div className="bos-aktivite">Henüz bir hareket yok.</div>
                )}
            </div>
        </div>

        <div className="sekmeler">
            <button className={aktifSekme === 'izlenecek' ? 'aktif' : ''} onClick={() => setAktifSekme('izlenecek')}>🎬 İzlenecekler</button>
            <button className={aktifSekme === 'izlendi' ? 'aktif' : ''} onClick={() => setAktifSekme('izlendi')}>✅ İzlediklerim</button>
            <button className={aktifSekme === 'okunacak' ? 'aktif' : ''} onClick={() => setAktifSekme('okunacak')}>📚 Okunacaklar</button>
            <button className={aktifSekme === 'okundu' ? 'aktif' : ''} onClick={() => setAktifSekme('okundu')}>✔️ Okuduklarım</button>
            {parseInt(profilId) === parseInt(benimId) && (
                <button 
                    className={aktifSekme === 'ozelliste' ? 'aktif' : ''} 
                    onClick={() => setAktifSekme('ozelliste')}
                >
                    📑 Özel Listeler
                </button>
            )}
        </div>

        <div className="liste-icerik">
            {aktifSekme === 'izlenecek' && <ListeGoster items={veri.izlenecekler} />}
            {aktifSekme === 'izlendi' && <ListeGoster items={veri.izlediklerim} />}
            {aktifSekme === 'okunacak' && <ListeGoster items={veri.okunacaklar} />}
            {aktifSekme === 'okundu' && <ListeGoster items={veri.okuduklarim} />}
            {aktifSekme === 'ozelliste' && (
            <div className="ozel-listeler-grid">
                
                {parseInt(profilId) === parseInt(benimId) && (
                    <div className="ozel-liste-kart yeni-liste-kart">
                        {listeOlusturmaModu ? (
                            <div className="liste-form-kucuk">
                                <input 
                                    type="text" 
                                    placeholder="Liste Adı..." 
                                    value={yeniListeAdi} 
                                    onChange={e => setYeniListeAdi(e.target.value)}
                                    autoFocus
                                />
                                <div style={{display:'flex', gap:'5px', marginTop:'5px'}}>
                                    <button onClick={listeOlustur} style={{background:'green', color:'white', flex:1}}>✓</button>
                                    <button onClick={()=>setListeOlusturmaModu(false)} style={{background:'red', color:'white', flex:1}}>X</button>
                                </div>
                            </div>
                        ) : (
                            <div onClick={() => setListeOlusturmaModu(true)} style={{height:'100%', display:'flex', flexDirection:'column', justifyContent:'center', alignItems:'center'}}>
                                <div className="art-ikon" style={{fontSize:'3rem', color:'#1877f2'}}>+</div>
                                <span>Yeni Liste Oluştur</span>
                            </div>
                        )}
                    </div>
                )}

                {ozelListeler.map(liste => (
                    <div 
                        key={liste.id} 
                        className="ozel-liste-kart" 
                        onClick={() => navigate(`/liste/${liste.id}`, { state: { baslik: liste.baslik } })}
                    >
                        <div className="klasor-ikon">📁</div>
                        <h4>{liste.baslik}</h4>
                        <span style={{fontSize:'0.8rem', color:'#666'}}>{new Date(liste.olusturulmaTarihi).toLocaleDateString()}</span>
                    </div>
                ))}
            </div>
        )}
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