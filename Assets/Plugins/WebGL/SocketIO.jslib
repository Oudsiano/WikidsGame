mergeInto(LibraryManager.library, {
    /**
     * Инициализирует соединение с Unity-колбэками
     */
    InitializeSocketWithCallbacks: function () {
        if (!window.clientConnector) {
            console.error("clientConnector not found");
            return;
        }

        // Инициализируем clientConnector без параметров
        window.clientConnector.initialize();
    },

    /**
     * Регистрирует Unity-колбэк для компонента
     */
    RegisterUnityCallback: function (componentNamePtr, methodNamePtr, callbackFuncPtr) {
        if (!window.clientConnector) {
            console.error("clientConnector not found");
            return;
        }
        
        const componentName = UTF8ToString(componentNamePtr);
        const methodName = UTF8ToString(methodNamePtr);
        
        // Убедимся что структура modules существует
        if (!window.unityCallbacks) {
            window.unityCallbacks = {};
        }
        
        if (!window.unityCallbacks[componentName]) {
            window.unityCallbacks[componentName] = {};
        }
        
        // Регистрируем функцию обратного вызова
        window.unityCallbacks[componentName][methodName] = function(jsonData) {
            const bufferSize = lengthBytesUTF8(jsonData) + 1;
            const buffer = _malloc(bufferSize);
            stringToUTF8(jsonData, buffer, bufferSize);
            dynCall_vi(callbackFuncPtr, buffer);
            _free(buffer);
        };
        
        // Обновляем модули в callbackManager
        window.clientConnector.initialize({ modules: window.unityCallbacks });
    },

    /**
     * Отправляет сообщение через Unity API clientConnector.unity.namespace.method
     */
    SendWebGLMessage: function (componentNamePtr, methodNamePtr, jsonDataPtr) {
        if (!window.clientConnector || !window.clientConnector.unity) {
            console.error("clientConnector.unity not available");
            return;
        }
        
        const componentName = UTF8ToString(componentNamePtr);
        const methodName = UTF8ToString(methodNamePtr);
        const jsonData = UTF8ToString(jsonDataPtr);
        
        if (!window.clientConnector.unity[componentName]) {
            console.error(`Unity component ${componentName} not found`);
            return;
        }
        
        if (!window.clientConnector.unity[componentName][methodName]) {
            console.error(`Unity method ${componentName}.${methodName} not found`);
            return;
        }
        
        // Вызываем метод из прокси для Unity
        // Прокси автоматически преобразует JSON-строку в объект
        window.clientConnector.unity[componentName][methodName](jsonData);
    },

    /**
     * Закрывает соединение и освобождает ресурсы
     */
    DestroySocket: function () {
        if (window.clientConnector && window.clientConnector.destroy) {
            window.clientConnector.destroy();
        }
        
        if (window.unityCallbacks) {
            window.unityCallbacks = null;
        }
    },

    TestJSLib: function () {
    console.log("JSLib works!");
     if (!window.clientConnector) {
        console.error(" clientConnector is NOT defined at TestJSLib call time.");
    } else {
        console.log("clientConnector exists");
    }
    },

    ConfirmRoom: function () {
    if (!window.clientConnector || !window.clientConnector.room) {
        console.error("clientConnector.room not available");
        return;
    }
    window.clientConnector.room.confirm();
    console.log("[AUTO] Sending room.confirm()");
    },

    FastJoinToRoom: function () {
    console.log("FastJoinToRoom");
     if (!window.clientConnector) {
        console.error(" clientConnector is NOT defined at TestJSLib call time.");
    } else {
        window.clientConnector.room.fastJoin();
         console.log("FastJoinToRoom is succesful!");
    }
    },

    /**
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
