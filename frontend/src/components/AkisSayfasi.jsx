import React, { useEffect, useState } from 'react';
import axios from 'axios';
import Navbar from './Navbar';
import AktiviteKarti from './AktiviteKarti';

const AkisSayfasi = () => {
  const [aktiviteler, setAktiviteler] = useState([]);
  const [sayfa, setSayfa] = useState(1);
  const [yukleniyor, setYukleniyor] = useState(false);
  const PORT = "44321";

  // Giriş yapan kullanıcıyı al (Kendi akışını görmek için)
  const kayitliKullanici = JSON.parse(localStorage.getItem('kullanici'));
  const benimId = kayitliKullanici ? kayitliKullanici.id : 1; 

  const veriGetir = (sayfaNo) => {
    setYukleniyor(true);
    // Backend'e hem kullaniciId hem sayfa numarasını gönderiyoruz
    axios.get(`https://localhost:${PORT}/api/Akis?kullaniciId=${benimId}&sayfa=${sayfaNo}`)
      .then(response => {
        if (sayfaNo === 1) {
            setAktiviteler(response.data);
        } else {
            // Yeni sayfayı eskilerin altına ekle (Infinite Scroll mantığı)
            setAktiviteler(prev => [...prev, ...response.data]);
        }
        setYukleniyor(false);
      })
      .catch(error => {
        console.error(error);
        setYukleniyor(false);
      });
  };

  useEffect(() => {
    veriGetir(1); // İlk açılış
  }, []);

  const dahaFazlaYukle = () => {
    const sonrakiSayfa = sayfa + 1;
    setSayfa(sonrakiSayfa);
    veriGetir(sonrakiSayfa);
  };

  return (
    <div>
        <Navbar />
        <div className="akis-container" style={{maxWidth:'600px', margin:'0 auto', padding:'20px'}}>
            {aktiviteler.length === 0 && !yukleniyor ? (
                <div style={{textAlign:'center', marginTop:'50px', color:'#777'}}>
                    <h3>Henüz bir aktivite yok.</h3>
                    <p>Arkadaşlarını takip etmeye başla veya içerikleri oyla!</p>
                </div>
            ) : (
                aktiviteler.map(act => (
                    <AktiviteKarti key={act.id} aktivite={act} />
                ))
            )}

            {/* Daha Fazla Yükle Butonu */}
            {aktiviteler.length > 0 && (
                <div style={{textAlign: 'center', margin: '20px'}}>
                    <button 
                        onClick={dahaFazlaYukle} 
                        disabled={yukleniyor}
                        style={{
                            padding: '10px 25px', 
                            cursor: 'pointer', 
                            backgroundColor: '#e4e6eb', 
                            border:'none', 
                            borderRadius:'6px', 
                            fontWeight:'bold', 
                            color:'#050505'
                        }}
                    >
                        {yukleniyor ? 'Yükleniyor...' : 'Daha Fazla Göster 👇'}
                    </button>
                </div>
            )}
        </div>
    </div>
  );
};

export default AkisSayfasi;