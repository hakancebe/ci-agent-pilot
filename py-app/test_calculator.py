from calculator import add, multiply


def test_add_iki_sayiyi_toplar():
    assert add(2, 3) == 5


def test_multiply_iki_sayiyi_carpar():
    assert multiply(2, 3) == 6
