import json

class DialogueManager:
    pass

# def get_dialogue(dialogue_letter, dialogue_index):
#     dialogue_key = f"Dialogue_{dialogue_letter}_Sen_{dialogue_index}"
#     # set the dialogue to find by searching through json and try find dialogue_letter and dialogue index, will be surrounded by constant Dialogue_{dialogueletter}_Sen_{dialogue_index}
#     # if both exist return the dialogue aka the value
#     with open("static/language.json", mode="r") as language_file:
#         language_data = json.load(language_file)
#         return language_data["en"][dialogue_key]