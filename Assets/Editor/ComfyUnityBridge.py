import json
import urllib.request
import urllib.parse
import time
import os
import sys
import shutil

# --- 설정 정보 ---
COMFYUI_SERVER_ADDR = "127.0.0.1:8188"
COMFYUI_OUTPUT_PATH = r"c:\Users\rlack\Desktop\AI\ComfyUI\ComfyUI\output"
UNITY_ASSET_PATH = r"c:\Users\rlack\Desktop\git\VampireECS\Assets\02_Art\Sprites\Icons"

# 사용자가 제공한 API Format Workflow (SDXL Lightning 전용)
WORKFLOW_JSON = """
{
  "103": {
    "inputs": {
      "ckpt_name": "Juggernaut_RunDiffusionPhoto2_Lightning_4Steps.safetensors"
    },
    "class_type": "CheckpointLoaderSimple"
  },
  "118": {
    "inputs": {
      "value": "minimalist simple 2d flat vector silhouette, solid shape, no detail, no lines"
    },
    "class_type": "PrimitiveStringMultiline"
  },
  "119": {
    "inputs": {
      "value": "photo, realistic, 3d, render, shadow, lighting, texture, gradient, grey, metallic, wood, shading, perspective"
    },
    "class_type": "PrimitiveStringMultiline"
  },
  "104": {
    "inputs": {
      "text": ["118", 0],
      "clip": ["103", 1]
    },
    "class_type": "CLIPTextEncode"
  },
  "105": {
    "inputs": {
      "text": ["119", 0],
      "clip": ["103", 1]
    },
    "class_type": "CLIPTextEncode"
  },
  "107": {
    "inputs": {
      "width": 1024,
      "height": 1024,
      "batch_size": 1
    },
    "class_type": "EmptyLatentImage"
  },
  "106": {
    "inputs": {
      "seed": 0,
      "steps": 8,
      "cfg": 1.5,
      "sampler_name": "dpmpp_sde",
      "scheduler": "karras",
      "denoise": 1,
      "model": ["103", 0],
      "positive": ["104", 0],
      "negative": ["105", 0],
      "latent_image": ["107", 0]
    },
    "class_type": "KSampler"
  },
  "108": {
    "inputs": {
      "samples": ["106", 0],
      "vae": ["103", 2]
    },
    "class_type": "VAEDecode"
  },
  "124": {
    "inputs": {
      "filename_prefix": "UnityIcon",
      "images": ["108", 0]
    },
    "class_type": "SaveImage"
  }
}
"""

def queue_prompt(prompt):
    p = {"prompt": prompt}
    data = json.dumps(p).encode('utf-8')
    req = urllib.request.Request(f"http://{COMFYUI_SERVER_ADDR}/prompt", data=data)
    with urllib.request.urlopen(req) as f:
        return json.loads(f.read().decode('utf-8'))

def check_history(prompt_id):
    with urllib.request.urlopen(f"http://{COMFYUI_SERVER_ADDR}/history/{prompt_id}") as f:
        return json.loads(f.read().decode('utf-8'))

def generate_icon(positive_prompt, filename):
    print(f"--- ComfyUI로 아이콘 생성 시작: {positive_prompt} ---")
    
    workflow = json.loads(WORKFLOW_JSON)
    
    # 프롬프트 수정 (Node 118)
    workflow["118"]["inputs"]["value"] = positive_prompt
    # 시드값 랜덤화 (Node 106 & 90)
    new_seed = int(time.time())
    
    workflow["106"]["inputs"]["seed"] = new_seed
    # 파일 이름 접두사 수정 (Node 124)
    workflow["124"]["inputs"]["filename_prefix"] = "UnityBridge_" + filename

    # 큐에 추가
    response = queue_prompt(workflow)
    prompt_id = response['prompt_id']
    print(f"프롬프트 대기열 추가됨 (ID: {prompt_id})")

    # 완료 대기 (간단한 폴링 방식)
    while True:
        history = check_history(prompt_id)
        if prompt_id in history:
            print("생성 완료!")
            # 결과 파일 이름 추출 (Node 124: SaveImage)
            output_info = history[prompt_id]['outputs']['124']['images'][0]
            generated_filename = output_info['filename']
            break
        time.sleep(1)

    # 파일 이동
    source = os.path.join(COMFYUI_OUTPUT_PATH, generated_filename)
    target = os.path.join(UNITY_ASSET_PATH, f"{filename}.png")
    
    if not os.path.exists(UNITY_ASSET_PATH):
        os.makedirs(UNITY_ASSET_PATH)
        
    shutil.move(source, target)
    print(f"에셋 이동 완료: {target}")

def check_server():
    try:
        with urllib.request.urlopen(f"http://{COMFYUI_SERVER_ADDR}/history/1") as f:
            return True
    except Exception as e:
        print(f"서버 연결 실패: {e}")
        return False

if __name__ == "__main__":
    # 인자가 '--check'인 경우 서버 상태만 확인
    if len(sys.argv) > 1 and sys.argv[1] == "--check":
        if check_server():
            print("SUCCESS: ComfyUI 서버가 정상 작동 중입니다.")
        else:
            print("FAIL: ComfyUI 서버에 연결할 수 없습니다.")
        sys.exit(0)

    # 일반 실행 시 인자 확인
    if len(sys.argv) < 3:
        print("--- ComfyUI Unity Bridge ---")
        print("사용법: python ComfyUnityBridge.py '프롬프트' '파일명'")
        print("상태 확인: python ComfyUnityBridge.py --check")
        print(f"현재 설정된 서버: {COMFYUI_SERVER_ADDR}")
        
        # 인자 없이 실행되어도 Antigravity가 죽지 않도록 상태 출력 후 정상 종료
        if check_server():
            print("서버 상태: 연결 가능")
        else:
            print("서버 상태: 연결 불가 (ComfyUI를 실행 중인지 확인하세요)")
    else:
        try:
            generate_icon(sys.argv[1], sys.argv[2])
        except Exception as e:
            print(f"오류 발생: {e}")
            sys.exit(1)
