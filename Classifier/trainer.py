import numpy as np
import os
import tensorflow as tf
import tf2onnx
from tensorflow.keras import layers, models, config
from sklearn.model_selection import train_test_split
import matplotlib.pyplot as plt

# Load categories
CATEGORIES = ["ambulance", "bee", "cactus", "donut", "sun", "car", "camera", "crown", "laptop", "knife", "rollerskates", "star"]
DATA_DIR = "quickdraw_data"
SAMPLES_PER_CLASS = 5000

config.set_image_data_format('channels_first')

# Load data
X, y = [], []
for i, category in enumerate(CATEGORIES):
    data = np.load(os.path.join(DATA_DIR, f"{category}.npy"))
    data = data[:SAMPLES_PER_CLASS]
    X.append(data)
    y += [i] * len(data)

X = np.concatenate(X, axis=0).reshape(-1, 1, 28, 28).astype("float32") / 255.0
y = np.array(y)

# Train/test split
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.2, random_state=42)

# Model
model = models.Sequential([
    layers.Conv2D(32, (3,3), activation='relu', input_shape=(1,28,28)),
    layers.MaxPooling2D(2,2),
    layers.Conv2D(64, (3,3), activation='relu'),
    layers.MaxPooling2D(2,2),
    layers.Flatten(),
    layers.Dense(128, activation='relu'),
    layers.Dense(len(CATEGORIES), activation='softmax')
])

model.compile(optimizer='adam',
              loss='sparse_categorical_crossentropy',
              metrics=['accuracy'])

# Train
model.fit(X_train, y_train, epochs=5, validation_data=(X_test, y_test))

# Save model
#model.backend.set_image_data_format("channels_first")
model.save("doodle_classifier.h5")

# Test one sample
def test_random():
    idx = np.random.randint(0, len(X_test))
    sample = X_test[idx]
    prediction = model.predict(sample.reshape(1, 1, 28, 28))
    print("Predicted:", CATEGORIES[np.argmax(prediction)])
    plt.imshow(sample.reshape(28, 28), cmap='gray')
    plt.show()

test_random()

model.output_names=['output']
# Define input signature (batch size is None for flexibility)
spec = (tf.TensorSpec([None, 1, 28, 28], tf.float32, name="input"),)

# Convert the Keras model to ONNX (no output_names needed)
onnx_model, _ = tf2onnx.convert.from_keras(model, input_signature=spec, opset=13)

# Save the ONNX model
with open("doodle_classifier.onnx", "wb") as f:
    f.write(onnx_model.SerializeToString())