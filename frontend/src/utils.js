export const zamanHesapla = (tarih) => {
    const saniye = Math.floor((new Date() - new Date(tarih)) / 1000);
    let aralik = saniye / 31536000;
  
    if (aralik > 1) return Math.floor(aralik) + " yıl önce";
    aralik = saniye / 2592000;
    if (aralik > 1) return Math.floor(aralik) + " ay önce";
    aralik = saniye / 86400;
    if (aralik > 1) return Math.floor(aralik) + " gün önce";
    aralik = saniye / 3600;
    if (aralik > 1) return Math.floor(aralik) + " saat önce"; 
    aralik = saniye / 60;
    if (aralik > 1) return Math.floor(aralik) + " dakika önce";
    return "Az önce";
};