mergeInto(LibraryManager.library, {
    /
     * Инициализирует соединение с Unity-колбэками
     */
    InitializeSocket: function (getProfileCallback, profileReceivedCallback) {
        function safeUnityCallback(callback, jsonData) {
            try {
                const bufferSize = lengthBytesUTF8(jsonData) + 1;
                const buffer = _malloc(bufferSize);
                stringToUTF8(jsonData, buffer, bufferSize);
                dynCall_vi(callback, buffer);
                _free(buffer);
            } catch (e) {
                console.error("Unity callback error:", e);
            }
        }

        // Регистрируем Unity-колбэки
        const unityCallbacks = {
            ProfileModule: {
                getProfile: function(jsonData) {
                    safeUnityCallback(getProfileCallback, jsonData);
                },
                onProfileReceived: function(jsonData) {
                    safeUnityCallback(profileReceivedCallback, jsonData);
                }
            }
        };

        // Проверяем доступность clientConnector
        if (!window.clientConnector) {
            console.error("clientConnector not found");
            return;
        }

        // Инициализируем clientConnector с нашими колбэками
        window.clientConnector.initialize({ modules: unityCallbacks });
    },

    /
     * Запрашивает профиль пользователя
     */
    RequestProfile: function (userIdPtr) {
        if (!window.clientConnector || !window.clientConnector.getProfile) {
            console.error("clientConnector.getProfile not available");
            return;
        }
        
        const userId = UTF8ToString(userIdPtr);
        window.clientConnector.getProfile({ userId: userId });
    },

    /
     * Закрывает соединение и освобождает ресурсы
     */
    DestroySocket: function () {
        if (window.clientConnector && window.clientConnector.destroy) {
            window.clientConnector.destroy();
        }
    },

    /
     * Проверяет инициализирован ли clientConnector
     * @returns {number} 1 если инициализирован, иначе 0
     */
    IsSocketInitialized: function () {
        if (!window.clientConnector) {
            return 0;
        }
        return window.clientConnector.isInitialized() ? 1 : 0;
    }
});