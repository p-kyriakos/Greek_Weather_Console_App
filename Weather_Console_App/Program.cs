using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    // reuse τον client για να μην εξαντλουμε τα sockets
    private static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        // utf8 για να μην βγαινουν τα ελληνικα ερωτηματικα
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("--- ΕΛΛΗΝΙΚΟΣ ΚΑΙΡΟΣ ---");
        Console.WriteLine();


        // loop για να ζηταει πολεις συνεχόμενα
        while (true)
        {
            Console.Write("Εισάγετε μια πόλη της Ελλάδας (ή 'ΤΕΛΟΣ' για έξοδο): ");
            string? city = Console.ReadLine()?.Trim();

            // εξοδος αν γραψει ΤΕΛΟΣ
            if (string.Equals(city, "ΤΕΛΟΣ", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("\nΗ εφαρμογή τερματίστηκε.");
                break;
            }

            // παρακαμψη αν πατησε σκετο enter
            if (string.IsNullOrWhiteSpace(city))
            {
                Console.WriteLine("Δεν εισάγατε πόλη. Προσπαθήστε ξανά.\n");
                continue;
            }

            try
            {
                //------------------ 1. βρες πολη geocoding api
                string encodedCity = Uri.EscapeDataString(city);
                string geocodingUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={encodedCity}&count=5&language=el&format=json&countryCode=GR";

                string geocodingResponse = await client.GetStringAsync(geocodingUrl);
                using JsonDocument geocodingJson = JsonDocument.Parse(geocodingResponse);

                // ελεγχος αν γυρισε κενη λιστα
                if (!geocodingJson.RootElement.TryGetProperty("results", out JsonElement results) || results.GetArrayLength() == 0)
                {
                    Console.WriteLine("\nΔεν βρέθηκε η πόλη. Παρακαλώ δοκιμάστε ξανά.\n");
                    continue;
                }

                // παιρνουμε το 1ο αποτελεσμα
                var location = results[0];
                double latitude = location.GetProperty("latitude").GetDouble();
                double longitude = location.GetProperty("longitude").GetDouble();
                string actualCityName = location.GetProperty("name").GetString() ?? city;






                // ----------------------------2. καλεσμα weather api
                // invariantculture για να μπει τελεια αντι για κομμα
                string latStr = latitude.ToString(CultureInfo.InvariantCulture);
                string lonStr = longitude.ToString(CultureInfo.InvariantCulture);
                string weatherUrl = $"https://api.open-meteo.com/v1/forecast?latitude={latStr}&longitude={lonStr}&current=temperature_2m,relative_humidity_2m,wind_speed_10m,weather_code&temperature_unit=celsius&wind_speed_unit=kmh";

                string weatherResponse = await client.GetStringAsync(weatherUrl);
                using JsonDocument weatherJson = JsonDocument.Parse(weatherResponse);

                JsonElement current = weatherJson.RootElement.GetProperty("current");






                //------------------ 3. διαβασμα δεδομενων
                double temperature = current.GetProperty("temperature_2m").GetDouble();
                int humidity = current.GetProperty("relative_humidity_2m").GetInt32();
                double windSpeed = current.GetProperty("wind_speed_10m").GetDouble();
                int weatherCode = current.GetProperty("weather_code").GetInt32();

                // μετατροπη κωδικου σε κειμενο
                string weatherDescription = GetWeatherDescription(weatherCode);




                //-------------------------- 4. εμφανιση
                Console.WriteLine("\n------ ΚΑΙΡΙΚΑ ΣΤΟΙΧΕΙΑ ------");
                Console.WriteLine($"Πόλη: {actualCityName}");
                Console.WriteLine($"Θερμοκρασία: {temperature:F1} °C");
                Console.WriteLine($"Υγρασία: {humidity}%");
                Console.WriteLine($"Αέρας: {windSpeed:F1} km/h");
                Console.WriteLine($"Συνθήκες: {weatherDescription}");
                Console.WriteLine("--------------------------------\n");
            }


            catch (HttpRequestException)
            {
                Console.WriteLine("\nΣφάλμα σύνδεσης. Ελέγξτε το ιντερνετ σας.\n");
            }
            catch (JsonException)
            {
                Console.WriteLine("\nΣφάλμα κατά την ανάγνωση των δεδομένων.\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nΚάτι πήγε στραβά: {ex.Message}\n");
            }
        }
    }

    // matching κωδικων καιρου σε κειμενο
    static string GetWeatherDescription(int code) => code switch
    {
        0 => "Καθαρός ουρανός",
        1 => "Κυρίως αίθριος",
        2 => "Μερική συννεφιά",
        3 => "Συννεφιασμένος",
        45 or 48 => "Ομίχλη",
        51 or 53 or 55 => "Ψιλόβροχο",
        56 or 57 => "Παγωμένο ψιλόβροχο",
        61 or 63 or 65 => "Βροχή",
        66 or 67 => "Παγωμένη βροχή",
        71 or 73 or 75 or 77 => "Χιονόπτωση",
        80 or 81 or 82 => "Μπόρες",
        85 or 86 => "Χιονομπόρες",
        95 => "Καταιγίδα",
        96 or 99 => "Καταιγίδα με χαλάζι",
        _ => "Άγνωστες συνθήκες"
    };
}