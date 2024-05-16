from codes import codes


def encode(input: str) -> str:
    current_state = "00"  # Начальное состояние
    encoded_code = ""  # Наш результат, куда мы записываем закодированные биты

    for bit in input:  # Проходимся по каждому биту в входной строке
        # Получаем код для текущего состояния и добавляем к результату
        encoded_code += codes[current_state][bit]["code"]
        # Обновляем состояние на основе текущего состояния и бита
        current_state = codes[current_state][bit]["state"]

    return encoded_code
