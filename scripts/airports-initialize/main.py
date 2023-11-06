import json
import asyncio
import asyncpg
import os
from translate import Translator

translator = Translator(to_lang="ru")

# Load the cache if it exists
if os.path.exists('translations.json'):
    with open('translations.json', 'r') as translations_file:
        translations = json.load(translations_file)
else:
    translations = {}


async def main():
    # Read the JSON file
    with open('airports.json', 'r') as airports_file:
        airports = json.load(airports_file)

    # Connect to the PostgreSQL database
    connection = await asyncpg.connect(
        database="",
        user="",
        password="",
        host="",
    )

    # Insert the airports into the table
    for i, airport in enumerate(airports, start=1):
        try:
            # Use the cached translation if it exists, otherwise translate and cache
            if airport['city'] in translations:
                russian_city = translations[airport['city']]
            else:
                russian_city = translator.translate(airport['city'])
                translations[airport['city']] = russian_city

            if airport['country'] in translations:
                russian_country = translations[airport['country']]
            else:
                russian_country = translator.translate(airport['country'])
                translations[airport['country']] = russian_country

            if airport['name'] in translations:
                russian_name = translations[airport['name']]
            else:
                russian_name = translator.translate(airport['name'])
                translations[airport['name']] = russian_name

            await connection.execute("""
                INSERT INTO public.airports (code, city, country, name)
                VALUES ($1, $2, $3, $4);
            """, airport['iata_code'], russian_city, russian_country, russian_name)

            print(f"{i}/{len(airports)}: inserted {airport['iata_code']} {russian_name}, {russian_city}, {russian_country}")

        except asyncpg.UniqueViolationError as e:
            # Handle duplicate entries (e.g., unique constraint violations)
            print(f"Error inserting {airport['iata_code']}: {str(e)}")

        # Save the cache on every iteration
        with open('translations.json', 'w') as file:
            json.dump(translations, file)

    # Close the connection
    await connection.close()


# Run the main function as an asynchronous task
asyncio.run(main())