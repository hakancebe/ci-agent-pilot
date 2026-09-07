// Kasıtlı hata: toplama yerine çıkarma yapıyor.
// Amaç, .NET dışı bir dilde hata analizi ölçmek.
function add(a, b) {
  return a - b;
}

module.exports = { add };
