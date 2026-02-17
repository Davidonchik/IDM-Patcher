using System;
using System.Collections.Generic;
using System.Globalization;

namespace IDMPatcherInstaller
{
    public static class Localization
    {
        private static Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>
        {
            // English (default)
            ["en"] = new Dictionary<string, string>
            {
                ["Title"] = "IDM Patcher Installer",
                ["ReadyToInstall"] = "Ready to install",
                ["Install"] = "Install",
                ["BrowsePath"] = "Browse IDM Path",
                ["Close"] = "Close",
                ["ClosingIDM"] = "Closing IDM if running...",
                ["CreatingDirectory"] = "Creating directory: {0}",
                ["ExtractingFiles"] = "Extracting embedded files...",
                ["CopyingFile"] = "Copying {0}...",
                ["PatchingIDM"] = "Patching IDM installation...",
                ["BackingUp"] = "Backing up original IDMan.exe...",
                ["ReplacingIDM"] = "Replacing IDMan.exe with launcher...",
                ["CopyingPatchFiles"] = "Copying patch files to IDM directory...",
                ["Retrying"] = "Retrying... ({0}/5)",
                ["WaitingUnlock"] = "Waiting for files to unlock... ({0}/5)",
                ["InstallComplete"] = "Installation complete!",
                ["InstallSuccess"] = "IDM Patcher has been installed successfully!",
                ["InstallSuccessMsg"] = "IDM Patcher has been installed successfully!\n\nYou can now close this window and launch IDM normally.\nIDM will automatically start with patch on every launch.",
                ["InstallCompleteTitle"] = "Installation Complete",
                ["ErrorTitle"] = "Error",
                ["ErrorAdmin"] = "Please run as Administrator!",
                ["ErrorInstallFailed"] = "Installation failed: {0}",
                ["ErrorIDMNotFound"] = "IDM not found!",
                ["ErrorIDMNotFoundMsg"] = "IDM is not installed at the default location:\n{0}\n\nPlease install IDM first or click 'Browse IDM Path' to specify custom location.",
                ["WarningFile"] = "WARNING: {0} not found, skipping...",
                ["KillingProcess"] = "Killing IDM process (PID: {0})...",
                ["PatchedSuccess"] = "IDM installation patched successfully!",
                ["Features"] = "===========================================\nIDM Patcher has been installed successfully!\nYou can now close this window and launch IDM normally.\nIDM will automatically start with patch.\n===========================================",
                ["SelectIDMPath"] = "Select IDM Installation Directory",
                ["IDMPathSet"] = "IDM path set to: {0}",
                ["Retry"] = "Retry",
                ["IDMStillRunning"] = "IDM is still running!\n\nPlease close IDM completely:\n• Right-click IDM tray icon → Exit\n• Or close IDM window\n\nThen click 'Retry' to continue installation."
            },
            
            // Russian
            ["ru"] = new Dictionary<string, string>
            {
                ["Title"] = "Установщик IDM Patcher",
                ["ReadyToInstall"] = "Готов к установке",
                ["Install"] = "Установить",
                ["BrowsePath"] = "Указать путь к IDM",
                ["Close"] = "Закрыть",
                ["ClosingIDM"] = "Закрытие IDM если запущен...",
                ["CreatingDirectory"] = "Создание директории: {0}",
                ["ExtractingFiles"] = "Извлечение встроенных файлов...",
                ["CopyingFile"] = "Копирование {0}...",
                ["PatchingIDM"] = "Патчинг установки IDM...",
                ["BackingUp"] = "Резервное копирование оригинального IDMan.exe...",
                ["ReplacingIDM"] = "Замена IDMan.exe на лаунчер...",
                ["CopyingPatchFiles"] = "Копирование файлов патча в директорию IDM...",
                ["Retrying"] = "Повтор... ({0}/5)",
                ["WaitingUnlock"] = "Ожидание разблокировки файлов... ({0}/5)",
                ["InstallComplete"] = "Установка завершена!",
                ["InstallSuccess"] = "IDM Patcher успешно установлен!",
                ["InstallSuccessMsg"] = "IDM Patcher успешно установлен!\n\nТеперь вы можете закрыть это окно и запустить IDM как обычно.\nIDM будет автоматически запускаться с патчем при каждом запуске.",
                ["InstallCompleteTitle"] = "Установка завершена",
                ["ErrorTitle"] = "Ошибка",
                ["ErrorAdmin"] = "Пожалуйста, запустите от имени администратора!",
                ["ErrorInstallFailed"] = "Установка не удалась: {0}",
                ["ErrorIDMNotFound"] = "IDM не найден!",
                ["ErrorIDMNotFoundMsg"] = "IDM не установлен в стандартном месте:\n{0}\n\nПожалуйста, сначала установите IDM или нажмите 'Указать путь к IDM' для выбора другого расположения.",
                ["WarningFile"] = "ВНИМАНИЕ: {0} не найден, пропускаем...",
                ["KillingProcess"] = "Завершение процесса IDM (PID: {0})...",
                ["PatchedSuccess"] = "Установка IDM успешно пропатчена!",
                ["Features"] = "===========================================\nIDM Patcher успешно установлен!\nТеперь вы можете закрыть это окно и запустить IDM.\nIDM будет автоматически запускаться с патчем.\n===========================================",
                ["SelectIDMPath"] = "Выберите директорию установки IDM",
                ["IDMPathSet"] = "Путь к IDM установлен: {0}",
                ["Retry"] = "Повторить",
                ["IDMStillRunning"] = "IDM все еще запущен!\n\nПожалуйста, закройте IDM полностью:\n• Правый клик на иконке IDM в трее → Выход\n• Или закройте окно IDM\n\nЗатем нажмите 'Повторить' для продолжения установки."
            },
            
            // German
            ["de"] = new Dictionary<string, string>
            {
                ["Title"] = "IDM Patcher Installer",
                ["ReadyToInstall"] = "Bereit zur Installation",
                ["Install"] = "Installieren",
                ["BrowsePath"] = "IDM-Pfad durchsuchen",
                ["Close"] = "Schließen",
                ["ClosingIDM"] = "IDM wird geschlossen, falls es läuft...",
                ["CreatingDirectory"] = "Verzeichnis erstellen: {0}",
                ["ExtractingFiles"] = "Eingebettete Dateien extrahieren...",
                ["CopyingFile"] = "Kopiere {0}...",
                ["PatchingIDM"] = "IDM-Installation wird gepatcht...",
                ["BackingUp"] = "Originale IDMan.exe wird gesichert...",
                ["ReplacingIDM"] = "IDMan.exe wird durch Launcher ersetzt...",
                ["CopyingPatchFiles"] = "Patch-Dateien werden in IDM-Verzeichnis kopiert...",
                ["Retrying"] = "Wiederhole... ({0}/5)",
                ["WaitingUnlock"] = "Warte auf Entsperrung der Dateien... ({0}/5)",
                ["InstallComplete"] = "Installation abgeschlossen!",
                ["InstallSuccess"] = "IDM Patcher wurde erfolgreich installiert!",
                ["InstallSuccessMsg"] = "IDM Patcher wurde erfolgreich installiert!\n\nSie können dieses Fenster jetzt schließen und IDM normal starten.\nIDM wird bei jedem Start automatisch mit Patch gestartet.",
                ["InstallCompleteTitle"] = "Installation abgeschlossen",
                ["ErrorTitle"] = "Fehler",
                ["ErrorAdmin"] = "Bitte als Administrator ausführen!",
                ["ErrorInstallFailed"] = "Installation fehlgeschlagen: {0}",
                ["ErrorIDMNotFound"] = "IDM nicht gefunden!",
                ["ErrorIDMNotFoundMsg"] = "IDM ist nicht am Standardspeicherort installiert:\n{0}\n\nBitte installieren Sie zuerst IDM oder klicken Sie auf 'IDM-Pfad durchsuchen', um einen benutzerdefinierten Speicherort anzugeben.",
                ["WarningFile"] = "WARNUNG: {0} nicht gefunden, wird übersprungen...",
                ["KillingProcess"] = "IDM-Prozess wird beendet (PID: {0})...",
                ["PatchedSuccess"] = "IDM-Installation erfolgreich gepatcht!",
                ["Features"] = "===========================================\nIDM Patcher wurde erfolgreich installiert!\nSie können dieses Fenster jetzt schließen und IDM starten.\nIDM wird automatisch mit Patch gestartet.\n===========================================",
                ["SelectIDMPath"] = "IDM-Installationsverzeichnis auswählen",
                ["IDMPathSet"] = "IDM-Pfad festgelegt auf: {0}"
            },
            
            // Spanish
            ["es"] = new Dictionary<string, string>
            {
                ["Title"] = "Instalador de IDM Patcher",
                ["ReadyToInstall"] = "Listo para instalar",
                ["Install"] = "Instalar",
                ["BrowsePath"] = "Buscar ruta de IDM",
                ["Close"] = "Cerrar",
                ["ClosingIDM"] = "Cerrando IDM si está en ejecución...",
                ["CreatingDirectory"] = "Creando directorio: {0}",
                ["ExtractingFiles"] = "Extrayendo archivos integrados...",
                ["CopyingFile"] = "Copiando {0}...",
                ["PatchingIDM"] = "Parcheando instalación de IDM...",
                ["BackingUp"] = "Respaldando IDMan.exe original...",
                ["ReplacingIDM"] = "Reemplazando IDMan.exe con lanzador...",
                ["CopyingPatchFiles"] = "Copiando archivos de parche al directorio de IDM...",
                ["Retrying"] = "Reintentando... ({0}/5)",
                ["WaitingUnlock"] = "Esperando desbloqueo de archivos... ({0}/5)",
                ["InstallComplete"] = "¡Instalación completa!",
                ["InstallSuccess"] = "¡IDM Patcher se ha instalado correctamente!",
                ["InstallSuccessMsg"] = "¡IDM Patcher se ha instalado correctamente!\n\nAhora puede cerrar esta ventana e iniciar IDM normalmente.\nIDM se iniciará automáticamente con el parche en cada inicio.",
                ["InstallCompleteTitle"] = "Instalación completa",
                ["ErrorTitle"] = "Error",
                ["ErrorAdmin"] = "¡Por favor, ejecute como administrador!",
                ["ErrorInstallFailed"] = "Instalación fallida: {0}",
                ["ErrorIDMNotFound"] = "¡IDM no encontrado!",
                ["ErrorIDMNotFoundMsg"] = "IDM no está instalado en la ubicación predeterminada:\n{0}\n\nPor favor, instale IDM primero o haga clic en 'Buscar ruta de IDM' para especificar una ubicación personalizada.",
                ["WarningFile"] = "ADVERTENCIA: {0} no encontrado, omitiendo...",
                ["KillingProcess"] = "Terminando proceso de IDM (PID: {0})...",
                ["PatchedSuccess"] = "¡Instalación de IDM parcheada correctamente!",
                ["Features"] = "===========================================\n¡IDM Patcher se ha instalado correctamente!\nAhora puede cerrar esta ventana e iniciar IDM.\nIDM se iniciará automáticamente con el parche.\n===========================================",
                ["SelectIDMPath"] = "Seleccionar directorio de instalación de IDM",
                ["IDMPathSet"] = "Ruta de IDM establecida en: {0}"
            },
            
            // French
            ["fr"] = new Dictionary<string, string>
            {
                ["Title"] = "Installateur IDM Patcher",
                ["ReadyToInstall"] = "Prêt à installer",
                ["Install"] = "Installer",
                ["BrowsePath"] = "Parcourir le chemin IDM",
                ["Close"] = "Fermer",
                ["ClosingIDM"] = "Fermeture d'IDM s'il est en cours d'exécution...",
                ["CreatingDirectory"] = "Création du répertoire: {0}",
                ["ExtractingFiles"] = "Extraction des fichiers intégrés...",
                ["CopyingFile"] = "Copie de {0}...",
                ["PatchingIDM"] = "Correction de l'installation IDM...",
                ["BackingUp"] = "Sauvegarde de l'IDMan.exe original...",
                ["ReplacingIDM"] = "Remplacement d'IDMan.exe par le lanceur...",
                ["CopyingPatchFiles"] = "Copie des fichiers de correctif dans le répertoire IDM...",
                ["Retrying"] = "Nouvelle tentative... ({0}/5)",
                ["WaitingUnlock"] = "En attente du déverrouillage des fichiers... ({0}/5)",
                ["InstallComplete"] = "Installation terminée!",
                ["InstallSuccess"] = "IDM Patcher a été installé avec succès!",
                ["InstallSuccessMsg"] = "IDM Patcher a été installé avec succès!\n\nVous pouvez maintenant fermer cette fenêtre et lancer IDM normalement.\nIDM démarrera automatiquement avec le correctif à chaque lancement.",
                ["InstallCompleteTitle"] = "Installation terminée",
                ["ErrorTitle"] = "Erreur",
                ["ErrorAdmin"] = "Veuillez exécuter en tant qu'administrateur!",
                ["ErrorInstallFailed"] = "Échec de l'installation: {0}",
                ["ErrorIDMNotFound"] = "IDM introuvable!",
                ["ErrorIDMNotFoundMsg"] = "IDM n'est pas installé à l'emplacement par défaut:\n{0}\n\nVeuillez d'abord installer IDM ou cliquez sur 'Parcourir le chemin IDM' pour spécifier un emplacement personnalisé.",
                ["WarningFile"] = "AVERTISSEMENT: {0} introuvable, ignoré...",
                ["KillingProcess"] = "Arrêt du processus IDM (PID: {0})...",
                ["PatchedSuccess"] = "Installation IDM corrigée avec succès!",
                ["Features"] = "===========================================\nIDM Patcher a été installé avec succès!\nVous pouvez maintenant fermer cette fenêtre et lancer IDM.\nIDM démarrera automatiquement avec le correctif.\n===========================================",
                ["SelectIDMPath"] = "Sélectionner le répertoire d'installation IDM",
                ["IDMPathSet"] = "Chemin IDM défini sur: {0}"
            },
            
            // Chinese Simplified
            ["zh"] = new Dictionary<string, string>
            {
                ["Title"] = "IDM Patcher 安装程序",
                ["ReadyToInstall"] = "准备安装",
                ["Install"] = "安装",
                ["BrowsePath"] = "浏览 IDM 路径",
                ["Close"] = "关闭",
                ["ClosingIDM"] = "正在关闭 IDM（如果正在运行）...",
                ["CreatingDirectory"] = "创建目录: {0}",
                ["ExtractingFiles"] = "提取嵌入文件...",
                ["CopyingFile"] = "复制 {0}...",
                ["PatchingIDM"] = "修补 IDM 安装...",
                ["BackingUp"] = "备份原始 IDMan.exe...",
                ["ReplacingIDM"] = "用启动器替换 IDMan.exe...",
                ["CopyingPatchFiles"] = "将补丁文件复制到 IDM 目录...",
                ["Retrying"] = "重试中... ({0}/5)",
                ["WaitingUnlock"] = "等待文件解锁... ({0}/5)",
                ["InstallComplete"] = "安装完成！",
                ["InstallSuccess"] = "IDM Patcher 已成功安装！",
                ["InstallSuccessMsg"] = "IDM Patcher 已成功安装！\n\n您现在可以关闭此窗口并正常启动 IDM。\nIDM 将在每次启动时自动使用补丁。",
                ["InstallCompleteTitle"] = "安装完成",
                ["ErrorTitle"] = "错误",
                ["ErrorAdmin"] = "请以管理员身份运行！",
                ["ErrorInstallFailed"] = "安装失败: {0}",
                ["ErrorIDMNotFound"] = "未找到 IDM！",
                ["ErrorIDMNotFoundMsg"] = "IDM 未安装在默认位置:\n{0}\n\n请先安装 IDM 或点击\"浏览 IDM 路径\"指定自定义位置。",
                ["WarningFile"] = "警告: 未找到 {0}，跳过...",
                ["KillingProcess"] = "终止 IDM 进程 (PID: {0})...",
                ["PatchedSuccess"] = "IDM 安装修补成功！",
                ["Features"] = "===========================================\nIDM Patcher 已成功安装！\n您现在可以关闭此窗口并启动 IDM。\nIDM 将自动使用补丁启动。\n===========================================",
                ["SelectIDMPath"] = "选择 IDM 安装目录",
                ["IDMPathSet"] = "IDM 路径设置为: {0}"
            }
        };

        private static string currentLanguage = "en";

        static Localization()
        {
            // Auto-detect system language
            string systemLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if (translations.ContainsKey(systemLang))
            {
                currentLanguage = systemLang;
            }
        }

        public static string Get(string key, params object[] args)
        {
            if (translations[currentLanguage].ContainsKey(key))
            {
                string text = translations[currentLanguage][key];
                if (args.Length > 0)
                {
                    return string.Format(text, args);
                }
                return text;
            }
            
            // Fallback to English
            if (translations["en"].ContainsKey(key))
            {
                string text = translations["en"][key];
                if (args.Length > 0)
                {
                    return string.Format(text, args);
                }
                return text;
            }
            
            return key;
        }
    }
}
