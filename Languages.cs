using System.Collections.Generic;

namespace ClassicExplorer
{
    public enum AppLanguage { EN, RU, UA, BE, PL, DE, FR, ES, IT, PT, ZH, JA, KO, AR, HI, TR, NL, SV, CS, RO }

    public static class Loc
    {
        // Изначально стоит английский!
        public static AppLanguage CurrentLang = AppLanguage.EN;

        private static Dictionary<string, string[]> strings = new Dictionary<string, string[]>()
        {
            { "MenuFile", new[] { "File", "Файл", "Файл", "Файл", "Plik", "Datei", "Fichier", "Archivo", "File", "Arquivo", "文件", "ファイル", "파일", "ملف", "फ़ाइल", "Dosya", "Bestand", "Arkiv", "Soubor", "Fișier" } },
            { "MenuEdit", new[] { "Edit", "Правка", "Правка", "Праўка", "Edycja", "Bearbeiten", "Édition", "Editar", "Modifica", "Editar", "编辑", "編集", "편집", "تعديل", "संपादित करें", "Düzenle", "Bewerken", "Redigera", "Upravit", "Editare" } },
            { "MenuView", new[] { "View", "Вид", "Вигляд", "Выгляд", "Widok", "Ansicht", "Affichage", "Ver", "Visualizza", "Exibir", "视图", "表示", "보기", "عرض", "देखें", "Görünüm", "Beeld", "Visa", "Zobrazit", "Vizualizare" } },
            { "MenuFav", new[] { "Favorites", "Избранное", "Улюблене", "Абранае", "Ulubione", "Favoriten", "Favoris", "Favoritos", "Preferiti", "Favoritos", "收藏夹", "お気に入り", "즐겨찾기", "المفضلة", "पसंदीदा", "Sık Kullanılanlar", "Favorieten", "Favoriter", "Oblíbené", "Favorite" } },
            { "MenuTools", new[] { "Tools", "Сервис", "Інструменти", "Сэрвіс", "Narzędzia", "Extras", "Outils", "Herramientas", "Strumenti", "Ferramentas", "工具", "ツール", "도구", "أدوات", "उपकरण", "Araçlar", "Extra", "Verktyg", "Nástroje", "Instrumente" } },
            { "MenuHelp", new[] { "Help", "Справка", "Довідка", "Даведка", "Pomoc", "Hilfe", "Aide", "Ayuda", "Guida", "Ajuda", "帮助", "ヘルプ", "도움말", "مساعدة", "मदद", "Yardım", "Help", "Hjälp", "Nápověda", "Ajutor" } },
            { "NewTab", new[] { "New Tab", "Новая вкладка", "Нова вкладка", "Новая ўкладка", "Nowa karta", "Neuer Tab", "Nouvel onglet", "Nueva pestaña", "Nuova scheda", "Nova aba", "新标签页", "新しいタブ", "새 탭", "علامة تبويب جديدة", "नया टैब", "Yeni Sekme", "Nieuw tabblad", "Ny flik", "Nová karta", "Filă nouă" } },
            { "CloseTab", new[] { "Close Tab", "Закрыть вкладку", "Закрити вкладку", "Закрыць укладку", "Zamknij kartę", "Tab schließen", "Fermer l'onglet", "Cerrar pestaña", "Chiudi scheda", "Fechar aba", "关闭标签页", "タブを閉じる", "탭 닫기", "إغلاق علامة التبويب", "टैब बंद करें", "Sekmeyi Kapat", "Tabblad sluiten", "Stäng flik", "Zavřít kartu", "Închide fila" } },
            { "Loading", new[] { "Loading...", "Загрузка...", "Завантаження...", "Загрузка...", "Ładowanie...", "Laden...", "Chargement...", "Cargando...", "Caricamento...", "Carregando...", "加载中...", "読み込み中...", "로딩 중...", "جار التحميل...", "लोड हो रहा है...", "Yükleniyor...", "Laden...", "Laddar...", "Načítání...", "Se încarcă..." } },
            { "Ready", new[] { "Ready", "Готово", "Готово", "Гатова", "Gotowe", "Bereit", "Prêt", "Listo", "Pronto", "Pronto", "就绪", "準備完了", "준비됨", "جاهز", "तैयार", "Hazır", "Gereed", "Klar", "Připraveno", "Gata" } },
            { "Back", new[] { "Back", "Назад", "Назад", "Назад", "Wstecz", "Zurück", "Retour", "Atrás", "Indietro", "Voltar", "返回", "戻る", "뒤로", "رجوع", "पीछे", "Geri", "Terug", "Tillbaka", "Zpět", "Înapoi" } },
            { "Forward", new[] { "Forward", "Вперед", "Вперед", "Наперад", "Dalej", "Vorwärts", "Suivant", "Adelante", "Avanti", "Avançar", "前进", "進む", "앞으로", "إلى الأمام", "आगे", "İleri", "Vooruit", "Framåt", "Vpřed", "Înainte" } },
            { "Stop", new[] { "Stop", "Остановить", "Зупинити", "Спыніць", "Zatrzymaj", "Stopp", "Arrêter", "Detener", "Ferma", "Parar", "停止", "中止", "중지", "إيقاف", "रोकें", "Durdur", "Stoppen", "Stoppa", "Zastavit", "Oprește" } },
            { "Refresh", new[] { "Refresh", "Обновить", "Оновити", "Абнавіць", "Odśwież", "Neu laden", "Actualiser", "Actualizar", "Aggiorna", "Atualizar", "刷新", "更新", "새로 고침", "تحديث", "ताज़ा करें", "Yenile", "Vernieuwen", "Uppdatera", "Obnovit", "Reîmprospătare" } },
            { "Home", new[] { "Home", "Домой", "Додому", "Дадому", "Strona główna", "Startseite", "Accueil", "Inicio", "Pagina iniziale", "Início", "主页", "ホーム", "홈", "الرئيسية", "होम", "Ana Sayfa", "Startpagina", "Hem", "Domů", "Acasă" } },
            { "Favorite", new[] { "Favorite", "В избранное", "Улюблене", "У абранае", "Ulubione", "Favorit", "Favori", "Favorito", "Preferito", "Favorito", "收藏", "お気に入り", "즐겨찾기", "مفضل", "पसंदीदा", "Favori", "Favoriet", "Favorit", "Oblíbené", "Favorit" } },
            { "Sound", new[] { "Sound", "Звук", "Звук", "Гук", "Dźwięk", "Ton", "Son", "Sonido", "Audio", "Som", "声音", "サウンド", "소리", "صوت", "ध्वनि", "Ses", "Geluid", "Ljud", "Zvuk", "Sunet" } },
            { "Downloads", new[] { "Downloads", "Загрузки", "Завантаження", "Загрузкі", "Pobrane", "Downloads", "Téléchargements", "Descargas", "Download", "Downloads", "下载", "ダウンロード", "다운로드", "التنزيلات", "डाउनलोड", "İndirilenler", "Downloads", "Nedladdningar", "Stažené", "Descărcări" } },
            { "Address", new[] { "Address:", "Адрес:", "Адреса:", "Адрас:", "Adres:", "Adresse:", "Adresse :", "Dirección:", "Indirizzo:", "Endereço:", "地址:", "アドレス:", "주소:", "العنوان:", "पता:", "Adres:", "Adres:", "Adress:", "Adresa:", "Adresă:" } },
            { "InPrivate", new[] { "InPrivate Browsing", "Режим InPrivate", "Режим InPrivate", "Рэжым InPrivate", "Tryb InPrivate", "InPrivate-Browsen", "Navigation InPrivate", "Navegación InPrivate", "Esplorazione InPrivate", "Navegação InPrivate", "InPrivate 浏览", "InPrivate ブラウズ", "InPrivate 브라우징", "استعراض InPrivate", "InPrivate ब्राउज़िंग", "InPrivate Gözatma", "InPrivate-navigatie", "InPrivate-surfning", "Procházení InPrivate", "Navigare InPrivate" } }
        };

        public static string Get(string key)
        {
            if (strings.ContainsKey(key)) {
                int index = (int)CurrentLang;
                if (index >= strings[key].Length) index = 0; // Если перевода нет, берем английский
                return strings[key][index];
            }
            return key; 
        }
    }
}