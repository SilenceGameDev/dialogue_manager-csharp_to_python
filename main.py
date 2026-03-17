from flask import Flask, render_template, request


app = Flask(__name__)

@app.route('/')
def home():
    return render_template('index.html')

# @app.route('/get_dialogue', methods=['POST'])
# def process_dialogue():
#     dialogue_letter = request.form['dialogue_letter']
#     dialogue_index = request.form['dialogue_index']
#
#     dialogue = dialogue_manager.get_dialogue(dialogue_letter, dialogue_index)
#
#     if dialogue:
#         return render_template('index.html', dialogue=dialogue)
#     else:
#         return render_template('index.html', error="Dialogue not found.")

if __name__ == '__main__':
    app.run(debug=True)