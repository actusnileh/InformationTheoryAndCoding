from encode import encode
from decode import decode


def main():
    print(f'Закодированный результат: {encode("10100")}')
    print(f'Декодированный результат: {decode("0011010111000011100001011100111011110101110011010100010111111011")}')


if __name__ == "__main__":
    main()
