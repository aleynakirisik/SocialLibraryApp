import React, { useEffect, useState } from 'react';
import axios from 'axios';
import Navbar from './Navbar';
import AktiviteKarti from './AktiviteKarti';

const AkisSayfasi = () => {
  const [aktiviteler, setAktiviteler] = useState([]);
  const [sayfa, setSayfa] = useState(1);
  const [yukleniyor, setYukleniyor] = useState(false);
  const PORT = "44321";

  const kayitliKullanici = JSON.parse(localStorage.getItem('kullanici'));
  const benimId = kayitliKullanici ? kayitliKullanici.id : 1; 

  const veriGetir = (sayfaNo) => {
    setYukleniyor(true);
    axios.get(`https://localhost:${PORT}/api/Akis?kullaniciId=${benimId}&sayfa=${sayfaNo}`)
      .then(response => {
        if (sayfaNo === 1) {
            setAktiviteler(response.data);
        } else {
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
    veriGetir(1); 
  }, []);

  const dahaFazlaYukle = () => {
    const sonrakiSayfa = sayfa + 1;
    setSayfa(sonrakiSayfa);
    veriGetir(sonrakiSayfa);
  };

return (
    <div>
        <Navbar />
        <div className="akis-container" style={{maxWidth:'600px', margin:'20px auto', padding:'0 15px'}}>

            <div style={{marginBottom:'20px', padding:'0 10px'}}>
                <h2 style={{fontSize:'1.5rem', color:'#333', marginBottom:'5px'}}>Akış</h2>
            </div>

            {aktiviteler.length === 0 && !yukleniyor ? (
                <div style={{textAlign:'center', marginTop:'50px', color:'#777', background:'white', padding:'40px', borderRadius:'12px', boxShadow:'0 2px 5px rgba(0,0,0,0.05)'}}>
                    <div style={{fontSize:'3rem', marginBottom:'10px'}}>📭</div>
                    <h3>Henüz bir aktivite yok.</h3>
                    <p>Arkadaşlarını takip etmeye başla veya içerikleri oyla!</p>
                </div>
            ) : (
                aktiviteler.map(act => (
                    <AktiviteKarti key={act.id} aktivite={act} />
                ))
            )}

            {aktiviteler.length > 0 && (
                <div style={{textAlign: 'center', margin: '30px 0'}}>
                    <button 
                        onClick={dahaFazlaYukle} 
                        disabled={yukleniyor}
                        style={{
                            padding: '12px 30px', 
                            cursor: 'pointer', 
                            background: 'white', 
                            border:'1px solid #ddd', 
                            borderRadius:'25px', 
                            fontWeight:'bold', 
                            color:'#1877f2',
                            boxShadow: '0 2px 5px rgba(0,0,0,0.05)',
                            transition: 'all 0.2s'
                        }}
                        onMouseOver={(e) => e.target.style.boxShadow = '0 4px 8px rgba(0,0,0,0.1)'}
                        onMouseOut={(e) => e.target.style.boxShadow = '0 2px 5px rgba(0,0,0,0.05)'}
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