# DrawMoar
Raster and vector graphics editor for creating cartoons

**Работающая версия лежит в ветке develop**

Запускать нужно проект DrawMoar, а не Demo.

Сейчас работает:

	Создание нового мультика (File->New) при размерах холста в пределах (криво, но мы поправим)
	Сохранение тукущего нарисованного в картинку в формате .png (тоже криво, но мы поправим)
	Рисование кистью
	Выбор цвета дя рисования
	Добавление на рисунок синих линий в рандомные места холста
	Реализовано создание мультика из кадров, но пока не прикручено к gui и использовать можно вручную из проекта Demo
	

**Детали реализации:**
    
        Меню:
        
            Первый пункт меню – “Файл”. Он включает в себя подпункты:
                Новый/Создать/New/Create
                Открыть
                Сохранить
                Сохранить как
                    MP4
                    AVI
                    
            Второй пункт – “Кадр”. Подпункты:
                Добавить (Открыть диалоговое окно с предложением добавить картинку в выбранных нами формата),
                Сохранить как
                    PNG
                    JPEG
                    **PSD
				Очистить кадр (удаляются все слои и остается один полностью чистый)
				
			Трерий пункт - "Инструменты". Подпункты (список будет дополняться и сортироваться):
				Снять выделение

  		
  
**Внутреннее представление:**
  
        Что из себя представляет наш собственный формат .drm?
        Это будет ПАПОЧКА (а не zip архив) с файлом манифеста, манифест будет представлять собой JSON файл, со 
		следующей структурой:
        {
	        “Frames”: [
		        {
			        Position: integer,
			        Duration: float,
			        Layers: [
				        {
					        ** “Position”: integer,
					        “Type”: “Vector” / ”Raster”,
					        “Source”: SVG file for “Vector” / Binary file for “Raster” type
				        },
			        ]
		        },
		        {
			        Position: integer,
			        Duration: float,
			        Layers: [
				        {
					        ** “Position”: integer,
					        “Type”: “Vector” / ”Raster”,
					        “Source”: SVG file for “Vector” / Binary file for “Raster” type
				        },
			        ]
		        }
	        ]
        }


(Старая диаграмма, на которой всё же понятна основная логика)
![UML](https://github.com/Anonymoar/DrawMoar/blob/master/UML%20Class%20Diagram.jpg)  
    

**Функциональные требования:**


Список фич:
  
    1. Содание нового мультика выбранного размера в пределах 256х144 - 3840х2160
    2. Создание нового пустого кадра
    3. Возмжность сделать копию выбранного кадра
    4. Удаление кадра
    5. Изменение порядка кадров
  
    6. Создание нового слоя пустого слоя, векторного или растрового
    6.1 Возможность переименовать слой
    7. Возможность сделать копию выбранного слоя
    8. Удаление слоя
    8.1 Изменение порядка слоёв
    8.2 Возможность сделать слой невидимым
    9. Растрирование векторного слоя
    10. Слияние слоёв по правилам:  R + R = R, V + R = R, V + V = V
  
    11. Сохранение промежуточного результата в \*your_cartoon\*.drm (наш собственный формат)
    12. Экспорт кадра в *name_of_your_image*.PNG
    13. Экспорт мультика в *name_of_your_cartoon*.mp4
  
    14. Добавление своего PNG или JPEG изображения на кадр (в качестве нового растрового слоя)
    15. Добавление фигуры из списка стандартных (в качестве нового векторного слоя)
    16. Преобразования над содержимым слоя (Translate, rotate, scale)
    17. Рисование кистью из набора стандартных кистей
    18. Пипетка
    19. Ластик
  
    20. Выбрать фрагмент слоя (select)
    21. Преобразования над выбранным фрагментов слоя (Translate, rotate, scale)
    23. Инструмент "Заливка"
  
    24. Отмена изменений в количестве n
    25. Добавление текста
    26. Использование текступ (styles в фотошопе) для векторных слоёв
    27. "Облака" для диалогов и мыслей, трансформирующиеся под текст
    28. Эффекты и фильтры для кадра
  
Дополнительные фичи которые мы возможно реализуем:

    29. *Выбор языка интерфекса (Russian, English, German)
    30. *Изменение размера мультика
    31. *Использование векторных линий, и вообще работа с ними
    32. *Рисование каплями (сложно объяснить)
    33. *Конструктор для добавления в стандартные своей фигуры
    34. *Выбор нумерации кадров с 0 или с 1 (по умолчанию с 0)
    35. **Рандомное перемешивание слоёв без возможности это откатить
    36. ***Слияние кадров
    37. ***Преобразование "синего круга" в "красный квадрат"
    38. ***Конструктор персонажа
    39. ***Работа с 3D
    40. *Речевое управление редактором (нарисуй красный круг радиуса 3  центре холста")
    42. ****Нейронки (рисование мультика по введенному тексту)
    43. ****Мультиплеер


**ИНФА ДЛЯ КОМАНДЫ (!!!)
Некомпилирующийся код никогда не должен попадать в репозиторий вообще, даже в ваших ветках. Локально - можно, здесь - нет.
(!!!!) Обратите внимание на ветку develop и на Color Picker**
