from codes import codes


def hamming_distance(input_1: str, input_2: str):
    # Подсчет количества отличающихся символов
    return sum(char1 != char2 for char1, char2 in zip(input_1, input_2))


def decode(input: str) -> str:
    input_list = []

    # Преобразую наш str в список (для удобства)
    for i in range(0, len(input), 2):
        input_list.append((input[i: i + 2]))

    edges = []
    length = len(input_list)

    # Рекурсивная функция для поиска всех возможных путей декодирования
    def find(state, i):
        if i >= length:  # Дошли до конца ввода.
            return

        for index in range(2):
            index_str = str(index)  # переводим индекс в строку
            next_code = codes[state][index_str]["code"]
            next_state = codes[state][index_str]["state"]
            metric = hamming_distance(
                list(input_list[i]), list(next_code)
            )  # Найдем дистанцию хамминга
            edges.append(
                {
                    "from": state,
                    "to": next_state,
                    "index": i,
                    "code": next_code,
                    "input": input_list[i],
                    "value": index_str,
                    "metric": metric,
                }
            )
            find(next_state, i + 1)  # Рекурсивно проходимся по следующим состояниям

    # Начинаем поиск с начального состояния "00"
    find("00", 0)

    paths = []
    path = []

    # Найдем все возможные пути
    for i, e in enumerate(edges):
        path.append(e)
        if int(e["index"]) == length - 1:  # Достигли конца
            paths.append(path.copy())  # Добавить путь к списку
            if len(edges) > i + 1:  # Если есть еще ребра
                for _ in range(length - int(edges[i + 1]["index"])):
                    path.pop()  # Удалить последнее ребро
    # Рассчитываем общую метрику для каждого пути
    for p in paths:
        total_metric = sum(int(edge["metric"]) for edge in p)
        p.append(total_metric)

    # Сортируем пути по метрике
    paths.sort(key=lambda x: x[-1])

    # Возвращаем декодированную строку, объединяя биты из самого лучшего пути
    return "".join(item["value"] for item in paths[0] if isinstance(item, dict))
