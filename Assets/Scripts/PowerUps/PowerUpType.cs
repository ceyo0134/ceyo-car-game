using UnityEngine;

// Ceyo Car Game'in kendi güç kartı sistemi.
// İsimler, açıklamalar ve görseller özgündür (başka bir oyundan kopyalanmamıştır).
// Her kartın "hissi" benzer bir oyun türüne (arcade yarış güç kartları) aittir,
// ama bu oyunda rakip araba olmadığı için etkiler parkur/engeller üzerine kuruludur.
public enum PowerUpType
{
    Dalga,   // Alan etkili darbe: önündeki engelleri/antiboost alanlarını devre dışı bırakır
    Tuzak,   // Arkana bir tuzak bırakır: ileride tekrar oraya gelirsen (ör. geri gidersen) yavaşlatır
    Itis,    // Yakın mesafe patlama: çevrendeki engelleri iter/yok eder
    Mermi,   // İleri doğru 3 atış: sırasıyla önündeki 3 engeli yok eder
    Gudum,   // Güdümlü mermi: en yakın engele kilitlenip onu yok eder
    Nitro,   // Ani hız patlaması (geri tuşuna basılıyken kullanılırsa fren/airbrake gibi çalışır)
    Kalkan,  // Belirli süre boyunca antiboost ve engel çarpmalarından etkilenmezsin
    Onarim   // Üzerindeki olumsuz efekti (yavaşlama vb.) anında temizler
}

public static class PowerUpData
{
    public struct Info
    {
        public string DisplayName;
        public string Description;
        public Color Color;
    }

    public static Info Get(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.Dalga:
                return new Info { DisplayName = "Dalga", Description = "Önündeki engelleri ve yavaşlatıcı alanları etkisiz hale getirir.", Color = new Color(0.2f, 0.6f, 1f) };
            case PowerUpType.Tuzak:
                return new Info { DisplayName = "Tuzak", Description = "Arkana bir tuzak bırakır, üzerine gelen yavaşlar.", Color = new Color(0.85f, 0.65f, 0.1f) };
            case PowerUpType.Itis:
                return new Info { DisplayName = "İtiş", Description = "Yakın çevredeki engelleri güçlü bir dalga ile iter.", Color = new Color(0.5f, 0.3f, 0.8f) };
            case PowerUpType.Mermi:
                return new Info { DisplayName = "Mermi", Description = "İleri doğru 3 atış yaparak yoldaki engelleri temizler.", Color = new Color(0.7f, 0.7f, 0.7f) };
            case PowerUpType.Gudum:
                return new Info { DisplayName = "Güdüm", Description = "En yakın engele kilitlenip onu yok eden güdümlü bir mermi fırlatır.", Color = new Color(0.9f, 0.2f, 0.2f) };
            case PowerUpType.Nitro:
                return new Info { DisplayName = "Nitro", Description = "Ani bir hız patlaması. Geri giderken kullanılırsa güçlü fren gibi çalışır.", Color = new Color(0.1f, 0.9f, 0.3f) };
            case PowerUpType.Kalkan:
                return new Info { DisplayName = "Kalkan", Description = "Belirli süre boyunca çarpışma ve yavaşlatıcı etkilerden korur.", Color = new Color(0.2f, 0.9f, 0.9f) };
            case PowerUpType.Onarim:
                return new Info { DisplayName = "Onarım", Description = "Üzerindeki olumsuz etkiyi anında temizler.", Color = new Color(1f, 0.85f, 0.2f) };
            default:
                return new Info { DisplayName = "Bilinmeyen", Description = "", Color = Color.white };
        }
    }
}
