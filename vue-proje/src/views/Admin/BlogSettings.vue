<template>
  <div class="admin-page-wrapper">
    <!-- ============ LİSTE GÖRÜNÜMÜ ============ -->
    <div v-if="!isEditing" class="blog-list-view">
      <div class="admin-page-header">
        <div>
          <h2 class="admin-title">Blog Yönetimi</h2>
          <p class="admin-subtitle">Yazı ve kitaplarınızı yönetin.</p>
        </div>
        <div style="display:flex;gap:0.75rem; align-items:center;">
          <label class="toggle-switch-inline" style="display:flex; align-items:center; gap:8px; cursor:pointer; background:rgba(255,255,255,0.05); padding:6px 12px; border-radius:8px; border:1px solid var(--admin-border);">
            <span style="color:var(--admin-text-main); font-size:0.9rem; font-weight:500;">Sitede Göster</span>
            <div class="toggle-switch" style="transform: scale(0.9); margin:0;">
              <input type="checkbox" v-model="pageVisibility" @change="saveVisibility">
              <span class="slider round"></span>
            </div>
          </label>
          <button @click="openTrash" class="admin-btn" style="background: rgba(239, 68, 68, 0.1); color: var(--admin-danger); border: 1px solid rgba(239, 68, 68, 0.2);">
            <i class="fas fa-trash-restore"></i> Çöp Kutusu
          </button>
          <button @click="showCategoryModal = true" class="admin-btn" style="background: var(--admin-surface); color: var(--admin-heading); border: 1px solid var(--admin-border);">
            <i class="fas fa-tags"></i> Kategoriler
          </button>
          <button @click="openEditor(null,'article')" class="admin-btn admin-btn-secondary">
            <i class="fas fa-pen-nib"></i> Kısa Yazı
          </button>
          <button @click="openEditor(null,'book')" class="admin-btn admin-btn-primary">
            <i class="fas fa-book"></i> Kitap Ekle
          </button>
        </div>
      </div>

      <div v-if="errorMsg" class="alert-error"><i class="fas fa-exclamation-circle"></i> {{ errorMsg }}</div>
      <div v-if="successMsg" class="alert-success"><i class="fas fa-check-circle"></i> {{ successMsg }}</div>
      <div v-if="loading" style="text-align:center;padding:3rem;color:var(--admin-primary);">
        <i class="fas fa-spinner fa-spin fa-2x"></i>
      </div>

      <div v-else-if="posts.length === 0" style="text-align:center;padding:3rem;background:var(--admin-surface);border:1px solid var(--admin-border);border-radius:var(--admin-radius-lg);">
        <i class="fas fa-book-open" style="font-size:3rem;color:var(--admin-text-muted);display:block;margin-bottom:1rem;"></i>
        <p style="color:var(--admin-text-muted);">Henüz hiç yazı eklenmedi.</p>
      </div>

      <div v-else style="display:grid;grid-template-columns:repeat(auto-fill,minmax(min(100%,300px),1fr));gap:1.5rem;">
        <div v-for="post in posts" :key="post.id" class="admin-card" style="padding:0;display:flex;flex-direction:column;overflow:hidden;">
          <!-- Kitap Tipi Rozeti -->
          <div :class="post.postType === 'book' ? 'card-thumb card-thumb-book' : 'card-thumb'"
               :style="post.postType === 'book' && post.bookColor ? `background:${post.bookColor}` : ''">
            <img v-if="post.coverImageUrl && post.postType !== 'book'" :src="getFullUrl(post.coverImageUrl)" style="width:100%;height:100%;object-fit:cover;" />
            <span v-else-if="post.icon && post.icon.includes('<svg')" v-safe-html="post.icon" class="svg-icon-wrapper" :style="`font-size:3.5rem;display:flex;align-items:center;justify-content:center;color:${post.postType === 'book' ? 'rgba(255,255,255,0.8)' : 'var(--admin-text-muted)'};`"></span>
            <i v-else-if="post.icon && (post.icon.includes('fa-') || post.icon.includes('ph-'))" :class="post.icon" :style="`font-size:3.5rem;color:${post.postType === 'book' ? 'rgba(255,255,255,0.8)' : 'var(--admin-text-muted)'};`"></i>
            <span v-else-if="post.icon" style="font-size:3.5rem;">{{ post.icon }}</span>
            <i v-else-if="post.postType === 'book'" class="fas fa-book" style="font-size:3.5rem;color:rgba(255,255,255,0.8);"></i>
            <i v-else class="fas fa-newspaper" style="font-size:3.5rem;color:var(--admin-text-muted);"></i>
            <span class="type-badge" :class="post.postType === 'book' ? 'badge-book' : 'badge-article'">
              {{ post.postType === 'book' ? '📚 Kitap' : '📝 Yazı' }}
            </span>
          </div>

          <div style="padding:1.25rem;flex:1;display:flex;flex-direction:column;">
            <div style="display:flex;justify-content:space-between;font-size:0.78rem;color:var(--admin-text-muted);margin-bottom:0.6rem;">
              <span>{{ formatDate(post.publishDate) }}</span>
              <div style="display:flex; gap:0.5rem;">
                <span v-if="post.category" style="background: rgba(255,51,0,0.1); color: var(--admin-primary); padding: 0.1rem 0.4rem; border-radius: 4px; border: 1px solid rgba(255,51,0,0.2);">
                  {{ post.category.name }}
                </span>
                <span v-if="post.tags && post.tags.length" style="color:var(--admin-primary);">{{ post.tags[0] }}</span>
              </div>
            </div>
            <h3 style="font-size:1.05rem;color:var(--admin-heading);margin-bottom:0.5rem;line-height:1.4;">{{ post.title }}</h3>
            <p style="color:var(--admin-text-muted);font-size:0.85rem;line-height:1.5;flex:1;">{{ post.summary }}</p>
            <div style="display:flex;justify-content:flex-end;gap:0.5rem;border-top:1px solid var(--admin-border);padding-top:1rem;margin-top:1rem;">
              <button @click="openEditor(post, post.postType)" class="admin-btn admin-btn-secondary" style="padding:0.4rem 0.9rem;font-size:0.82rem;">
                <i class="fas fa-pen"></i> Düzenle
              </button>
              <button @click="deletePost(post.id)" class="admin-btn" style="background:rgba(239,68,68,0.1);color:var(--admin-danger);border:1px solid rgba(239,68,68,0.2);padding:0.4rem 0.9rem;font-size:0.82rem;">
                <i class="fas fa-trash"></i> Sil
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Trash Modal -->
    <div v-if="showTrashModal" style="position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.8); z-index: 1000; display: flex; align-items: center; justify-content: center;">
      <div class="admin-card" style="width: 600px; max-width: 90%; max-height: 80vh; display: flex; flex-direction: column;">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
          <h3 style="font-size: 1.2rem; color: var(--admin-danger); margin: 0;"><i class="fas fa-trash-alt"></i> Çöp Kutusu</h3>
          <button @click="showTrashModal = false" style="background: transparent; border: none; color: var(--admin-text-muted); cursor: pointer; font-size: 1.2rem;">
            <i class="fas fa-times"></i>
          </button>
        </div>
        
        <div v-if="loadingTrash" style="text-align: center; padding: 2rem; color: var(--admin-primary);">
          <i class="fas fa-spinner fa-spin fa-2x"></i>
        </div>
        
        <div v-else-if="trashItems.length === 0" style="text-align: center; padding: 2rem; color: var(--admin-text-muted);">
          Çöp kutusu boş.
        </div>
        
        <div v-else style="overflow-y: auto;" class="admin-scroll">
          <ul style="list-style: none; padding: 0; margin: 0;">
            <li v-for="item in trashItems" :key="item.id" style="padding: 1rem; border-bottom: 1px solid var(--admin-border); display: flex; justify-content: space-between; align-items: center;">
              <div>
                <strong style="color: var(--admin-heading); display: block; margin-bottom: 0.25rem;">{{ item.title }}</strong>
                <span style="font-size: 0.8rem; color: var(--admin-text-muted);">{{ item.postType === 'book' ? '📚 Kitap' : '📝 Yazı' }}</span>
              </div>
              <div style="display: flex; gap: 0.5rem;">
                <button @click="restorePost(item.id)" class="admin-btn admin-btn-secondary" style="padding: 0.4rem 0.8rem; font-size: 0.8rem;">
                  <i class="fas fa-undo"></i> Kurtar
                </button>
                <button @click="hardDeletePost(item.id)" class="admin-btn" style="background: var(--admin-danger); color: white; border: none; padding: 0.4rem 0.8rem; font-size: 0.8rem;">
                  Kalıcı Sil
                </button>
              </div>
            </li>
          </ul>
        </div>
      </div>
    </div>

    <!-- Category Modal -->
    <div v-if="showCategoryModal" style="position: fixed; top: 0; left: 0; right: 0; bottom: 0; background: rgba(0,0,0,0.8); z-index: 1000; display: flex; align-items: center; justify-content: center;">
      <div class="admin-card" style="width: 400px; max-width: 90%;">
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem;">
          <h3 style="font-size: 1.2rem; color: var(--admin-heading); margin: 0;">Kategori Yönetimi</h3>
          <button @click="showCategoryModal = false" style="background: transparent; border: none; color: var(--admin-text-muted); cursor: pointer; font-size: 1.2rem;">
            <i class="fas fa-times"></i>
          </button>
        </div>
        
        <div style="margin-bottom: 1.5rem; max-height: 200px; overflow-y: auto;" class="admin-scroll">
          <ul style="list-style: none; padding: 0; margin: 0;">
            <li v-for="cat in categories" :key="cat.id" style="padding: 0.75rem; border-bottom: 1px solid var(--admin-border); display: flex; justify-content: space-between; align-items: center;">
              <span>{{ cat.name }}</span>
              <button @click="deleteCategory(cat.id)" style="background:none; border:none; color:var(--admin-danger); cursor:pointer;" title="Kategoriyi Sil">
                <i class="fas fa-trash"></i>
              </button>
            </li>
            <li v-if="categories.length === 0" style="padding: 0.75rem; color: var(--admin-text-muted); text-align: center;">Kategori bulunamadı.</li>
          </ul>
        </div>

        <form @submit.prevent="addCategory" style="display: flex; flex-direction: column; gap: 1rem;">
          <div style="display: flex; gap: 0.75rem;">
            <div style="flex: 1; min-width: 140px;">
              <IconPicker v-model="newCategoryIcon" mode="icon" />
            </div>
            <input type="text" v-model="newCategoryName" class="admin-input" placeholder="Yeni Kategori Adı [TR]" style="flex: 2; margin: 0;" required />
            <input type="text" v-model="newCategoryNameEn" class="admin-input" placeholder="Yeni Kategori Adı [EN]" style="flex: 2; margin: 0;" />
          </div>
          <button type="submit" class="admin-btn admin-btn-primary" style="justify-content: center;">Yeni Kategori Ekle</button>
        </form>
      </div>
    </div>

    <!-- ============ EDITOR GÖRÜNÜMÜ ============ -->
    <div v-else-if="isEditing" class="blog-editor-view">
      <div class="admin-page-header">
        <div>
          <div style="display:flex;align-items:center;gap:0.75rem;margin-bottom:0.5rem;">
            <span class="type-badge" :class="currentPost.postType === 'book' ? 'badge-book' : 'badge-article'" style="font-size:0.85rem;">
              {{ currentPost.postType === 'book' ? '📚 Kitap Modu' : '📝 Yazı Modu' }}
            </span>
          </div>
          <h2 class="admin-title">{{ currentPost.id ? 'Düzenle' : 'Yeni ' + (currentPost.postType === 'book' ? 'Kitap' : 'Yazı') }}</h2>
          <button @click="closeEditor" style="background:transparent;border:none;color:var(--admin-text-muted);cursor:pointer;display:flex;align-items:center;gap:0.5rem;margin-top:0.4rem;font-size:0.9rem;">
            <i class="fas fa-arrow-left"></i> Listeye Dön
          </button>
        </div>
        <div style="display: flex; gap: 10px;">
          <button @click="clearTranslations" class="admin-btn" style="background: rgba(239, 68, 68, 0.1); color: var(--admin-danger); border: 1px solid rgba(239, 68, 68, 0.2);">
            <i class="fas fa-eraser"></i> Çeviriyi Sil
          </button>
          <button v-if="currentPost.contentEn" @click="refineWithAI" class="admin-btn admin-btn-ai" :disabled="aiLoading" style="background: rgba(138, 43, 226, 0.1); color: #8a2be2; border-color: rgba(138, 43, 226, 0.3);">
            <i class="fas" :class="aiLoading ? 'fa-spinner fa-spin' : 'fa-user-secret'"></i> 
            {{ aiLoading ? 'Denetleniyor...' : 'Denetle ve Onar' }}
          </button>
          <button v-else @click="translateWithAI" class="admin-btn admin-btn-ai" :disabled="aiLoading">
            <i class="fas" :class="aiLoading ? 'fa-spinner fa-spin' : 'fa-magic'"></i> 
            {{ aiLoading ? 'Çevriliyor...' : '✨ AI ile Çevir' }}
          </button>
          <button @click="savePost" class="admin-btn admin-btn-primary" :disabled="saving">
            <i class="fas" :class="saving ? 'fa-spinner fa-spin' : 'fa-save'"></i>
            {{ saving ? 'Kaydediliyor...' : 'Kaydet' }}
          </button>
        </div>
      </div>

      <div v-if="errorMsg" class="alert-error"><i class="fas fa-exclamation-circle"></i> {{ errorMsg }}</div>

      <!-- ======= ORTAK ALANLAR ======= -->
      <div class="admin-grid-2-1">
        <div style="display:flex;flex-direction:column;gap:1.5rem;">

          <div class="admin-card" style="display:flex;flex-direction:column;gap:1.25rem;">
            <div class="admin-form-group">
              <label class="admin-label">Başlık [TR] *</label>
              <input type="text" v-model="currentPost.title" class="admin-input" placeholder="Yazı / Kitap başlığı" />
            </div>
            <div class="admin-form-group">
              <label class="admin-label">Başlık [EN]</label>
              <input type="text" v-model="currentPost.titleEn" class="admin-input" placeholder="Post / Book title" />
            </div>
            
            <div class="admin-form-group" style="position: relative;">
              <label class="admin-label">Kategori</label>
              
              <!-- Custom Dropdown Selector -->
              <div 
                class="admin-input" 
                style="display: flex; align-items: center; justify-content: space-between; cursor: pointer; user-select: none;"
                @click="showCategoryDropdown = !showCategoryDropdown"
              >
                <div style="display: flex; align-items: center; gap: 8px;">
                  <template v-if="currentPost.blogCategoryId && categories.find(c => c.id === currentPost.blogCategoryId)">
                    <i v-if="categories.find(c => c.id === currentPost.blogCategoryId).icon" :class="categories.find(c => c.id === currentPost.blogCategoryId).icon"></i>
                    <span>{{ categories.find(c => c.id === currentPost.blogCategoryId).name }}</span>
                  </template>
                  <span v-else style="color: var(--admin-text-muted);">Kategori Seçin</span>
                </div>
                <i class="fas fa-chevron-down" style="font-size: 0.8rem; color: var(--admin-text-muted);"></i>
              </div>
              
              <!-- Dropdown Content -->
              <div v-if="showCategoryDropdown" 
                   style="position: absolute; top: calc(100% + 4px); left: 0; right: 0; background: var(--admin-surface); border: 1px solid var(--admin-border); border-radius: 8px; z-index: 50; max-height: 250px; overflow-y: auto; box-shadow: 0 10px 30px rgba(0,0,0,0.5);">
                <div 
                  style="padding: 12px 16px; cursor: pointer; display: flex; align-items: center; gap: 8px; transition: background 0.2s;"
                  onmouseover="this.style.background='var(--admin-surface-hover)'"
                  onmouseout="this.style.background='transparent'"
                  @click="currentPost.blogCategoryId = null; showCategoryDropdown = false"
                >
                  <span style="color: var(--admin-text-muted);">Kategori Yok (Seçimi Temizle)</span>
                </div>
                <div 
                  v-for="cat in categories" :key="cat.id" 
                  style="padding: 12px 16px; cursor: pointer; display: flex; align-items: center; gap: 8px; transition: background 0.2s; border-top: 1px solid rgba(255,255,255,0.05);"
                  onmouseover="this.style.background='var(--admin-surface-hover)'"
                  onmouseout="this.style.background='transparent'"
                  @click="currentPost.blogCategoryId = cat.id; showCategoryDropdown = false"
                >
                  <i v-if="cat.icon" :class="cat.icon" style="color: var(--admin-primary); font-size: 1.1rem; width: 24px; text-align: center;"></i>
                  <span>{{ cat.name }}</span>
                </div>
              </div>

              <!-- Click outside overlay -->
              <div v-if="showCategoryDropdown" @click="showCategoryDropdown = false" style="position: fixed; inset: 0; z-index: 40;"></div>
            </div>

            <div class="admin-form-group">
              <label class="admin-label">Kısa Özet [TR]</label>
              <textarea v-model="currentPost.summary" class="admin-input" placeholder="Blog listesinde görünecek özet" style="min-height:80px;"></textarea>
            </div>
            <div class="admin-form-group">
              <label class="admin-label">Kısa Özet [EN]</label>
              <textarea v-model="currentPost.summaryEn" class="admin-input" placeholder="Summary for blog list" style="min-height:80px;"></textarea>
            </div>
          </div>

          <!-- ===== KİTAP MODU İÇERİK ===== -->
          <div v-if="currentPost.postType === 'book'" class="admin-card" style="display:flex;flex-direction:column;gap:1rem;">
            <div style="display:flex;align-items:center;gap:0.5rem;margin-bottom:0.25rem;">
              <i class="fas fa-book" style="color:var(--admin-primary);"></i>
              <label class="admin-label" style="margin:0;">Kitap İçeriği Editörü [TR]</label>
            </div>
            <AutoFormatter v-model="currentPost.content" />

            <div style="display:flex;align-items:center;gap:0.5rem;margin-bottom:0.25rem;margin-top:1rem;">
              <i class="fas fa-book" style="color:var(--admin-primary);"></i>
              <label class="admin-label" style="margin:0;">Kitap İçeriği Editörü [EN]</label>
            </div>
            <AutoFormatter v-model="currentPost.contentEn" />

            <!-- Otomatik İçindekiler Önizleme (Kitap İçin) -->
            <div v-if="bookToc.length > 0" style="background:var(--admin-surface);border:1px solid var(--admin-border);border-radius:8px;padding:1rem;">
              <div style="display:flex;align-items:center;gap:0.5rem;margin-bottom:0.75rem;">
                <i class="fas fa-list-ul" style="color:var(--admin-primary);"></i>
                <span style="font-weight:700;color:var(--admin-heading);font-size:0.9rem;">Otomatik Algılanan İçindekiler ({{ bookToc.length }} sayfa)</span>
              </div>
              <div v-for="(item, i) in bookToc" :key="i" style="display:flex;justify-content:space-between;padding:0.3rem 0;border-bottom:1px solid var(--admin-border);font-size:0.85rem;">
                <span :style="item.level === 3 ? 'padding-left:1.5rem;color:var(--admin-text-muted);' : 'color:var(--admin-heading);font-weight:600;'">
                  {{ item.level === 2 ? `${item.pageNum}. ` : '↳ ' }}{{ item.title }}
                </span>
                <span v-if="item.level === 2" style="color:var(--admin-text-muted);font-size:0.78rem;">Sayfa {{ item.pageNum }}</span>
              </div>
            </div>
          </div>

          <!-- ===== YAZI MODU İÇERİK ===== -->
          <div v-else class="admin-card" style="display:flex;flex-direction:column;gap:1rem;">
            <div style="display:flex;align-items:center;justify-content:space-between;">
              <label class="admin-label" style="margin:0;display:flex;align-items:center;gap:0.5rem;">
                <i class="fas fa-pen-nib" style="color:var(--admin-primary);"></i>
                Yazı İçeriği Editörü [TR]
              </label>
            </div>
            <textarea v-model="currentPost.content" class="admin-input" rows="12" placeholder="Kısa yazınızı buraya Markdown veya düz metin olarak yazabilirsiniz..."></textarea>

            <div style="display:flex;align-items:center;justify-content:space-between;margin-top:1rem;">
              <label class="admin-label" style="margin:0;display:flex;align-items:center;gap:0.5rem;">
                <i class="fas fa-pen-nib" style="color:var(--admin-primary);"></i>
                Yazı İçeriği Editörü [EN]
              </label>
            </div>
            <textarea v-model="currentPost.contentEn" class="admin-input" rows="12" placeholder="You can write your short post here..."></textarea>
          </div>

          <div class="admin-card">
            <div class="admin-form-group">
              <label class="admin-label">Pratik İpucu (ProTip) [TR]</label>
              <input type="text" v-model="currentPost.proTip" class="admin-input" placeholder="Dikkat çekilecek bir ipucu" />
            </div>
            <div class="admin-form-group">
              <label class="admin-label">Pratik İpucu (ProTip) [EN]</label>
              <input type="text" v-model="currentPost.proTipEn" class="admin-input" placeholder="ProTip..." />
            </div>
          </div>
        </div>

        <!-- SAĞ KOLON -->
        <div style="display:flex;flex-direction:column;gap:1.5rem;">

          <!-- Kitap Kapak Rengi (Sadece Kitap) -->
          <div v-if="currentPost.postType === 'book'" class="admin-card">
            <label class="admin-label" style="display:flex;align-items:center;gap:0.5rem;">
              <i class="fas fa-palette" style="color:var(--admin-primary);"></i>
              Kitap Kapak Rengi
            </label>
            <div style="display:flex;flex-wrap:wrap;gap:0.5rem;margin-bottom:1rem;">
              <div
                v-for="col in bookColors" :key="col"
                @click="currentPost.bookColor = col"
                :style="`background:${col};width:36px;height:36px;border-radius:6px;cursor:pointer;border:3px solid ${currentPost.bookColor===col ? '#fff' : 'transparent'};box-shadow:${currentPost.bookColor===col ? '0 0 0 2px var(--admin-primary)' : 'none'};transition:all .2s;`"
              ></div>
            </div>
            <div style="display:flex;align-items:center;gap:0.75rem;">
              <input type="color" v-model="currentPost.bookColor" style="width:48px;height:38px;border:none;background:none;cursor:pointer;padding:0;" />
              <input type="text" v-model="currentPost.bookColor" class="admin-input" style="flex:1;font-family:monospace;font-size:0.9rem;" placeholder="#1a1a2e" />
            </div>
            <div style="margin-top:1rem;">
              <div style="border-radius:10px;height:80px;display:flex;align-items:center;justify-content:center;font-weight:700;color:#fff;font-size:0.95rem;letter-spacing:1px;text-shadow:0 1px 3px rgba(0,0,0,0.5);"
                   :style="`background:${currentPost.bookColor || '#1a1a2e'}`">
                {{ currentPost.title || 'Kitap Başlığı' }}
              </div>
            </div>
          </div>

          <!-- Medya / Kapak Görseli -->
          <div class="admin-card" v-if="currentPost.postType === 'book'">
            <label class="admin-label" style="display:flex;align-items:center;gap:0.5rem;">
              <i class="fas fa-image" style="color:var(--admin-primary);"></i>
              Medya (Kapak Görseli)
            </label>
            <ImageUploader v-model="currentPost.coverImageUrl" label="Kapak Görseli Seç" />
            
            <div class="admin-form-group" style="margin-top:1.5rem;">
              <label class="admin-label">Ana İkon (Alternatif)</label>
              <IconPicker v-model="currentPost.icon" mode="emoji" />
            </div>
          </div>

          <!-- Etiketler -->
          <div class="admin-card">
            <div class="admin-form-group">
              <label class="admin-label">Etiketler (Virgülle)</label>
              <input type="text" v-model="rawTags" @input="updateTags" class="admin-input" placeholder="Vue, Frontend, JS" />
            </div>

            <div class="admin-form-group" style="margin-top:1rem;" v-if="currentPost.postType === 'book'">
              <label class="admin-label">Teknoloji İkonları</label>
              <div v-if="currentPost.techIcons && currentPost.techIcons.length" style="display:flex;flex-wrap:wrap;gap:6px;margin-bottom:10px;">
                <div v-for="(tag,idx) in currentPost.techIcons" :key="idx" style="background:var(--admin-border);padding:3px 10px;border-radius:4px;display:flex;align-items:center;gap:6px;font-size:0.82rem;">
                  <i v-if="tag.includes('|')" :class="tag.split('|')[0]"></i>
                  <span>{{ tag.includes('|') ? tag.split('|')[1] : tag }}</span>
                  <button @click.prevent="removeTechIcon(idx)" style="background:none;border:none;color:var(--admin-danger);cursor:pointer;line-height:1;">×</button>
                </div>
              </div>
              <div style="display:flex;gap:6px;flex-wrap:wrap;">
                <div style="flex:1; min-width:130px;"><IconPicker v-model="newTechIcon" mode="icon" /></div>
                <div style="display:flex; flex:2; min-width:160px; gap:6px;">
                  <input type="text" v-model="newTechName" placeholder="Teknoloji adı" class="admin-input" style="flex:1;margin:0;" @keyup.enter="addTechIcon" />
                  <button @click.prevent="addTechIcon" class="admin-btn admin-btn-secondary" style="height:48px;width:48px;padding:0;display:flex;align-items:center;justify-content:center;flex-shrink:0;"><i class="fas fa-plus"></i></button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, watch } from 'vue'
import api from '@/services/api'
import translationService from '@/services/translationService'
import ImageUploader from '@/components/ImageUploader.vue'
import IconPicker from '@/components/IconPicker.vue'
import AutoFormatter from '@/components/AutoFormatter.vue'
import swal from '@/utils/swal'
import { marked } from 'marked'
import hljs from 'highlight.js'
import 'highlight.js/styles/atom-one-dark.css'

// ─── State ───────────────────────────────────────────
const posts       = ref([])
const categories  = ref([])
const loading     = ref(true)

const pageVisibility = ref(true)
const seoData = ref(null)

const isEditing   = ref(false)
const saving      = ref(false)
const errorMsg    = ref('')
const successMsg  = ref('')
const currentPost = ref({})
const rawTags     = ref('')
const newTechIcon = ref('fa-brands fa-vuejs')
const newTechName = ref('')
const showCategoryModal = ref(false)
const showCategoryDropdown = ref(false)
const newCategoryName = ref('')
const newCategoryNameEn = ref('')
const newCategoryIcon = ref('')
const articleMode = ref('split')   // 'write' | 'preview' | 'split'
const articleTextarea = ref(null)
const bookToc     = ref([])
const aiLoading = ref(false)

const showTrashModal = ref(false)
const trashItems = ref([])
const loadingTrash = ref(false)

// ─── Kitap Kapak Renkleri ────────────────────────────
const bookColors = [
  '#1a1a2e', '#16213e', '#0f3460', '#533483',
  '#e94560', '#ff3b1d', '#f5a623', '#27ae60',
  '#2c3e50', '#8e44ad', '#2980b9', '#c0392b',
  '#1abc9c', '#d35400', '#34495e', '#7f8c8d'
]



// ─── Yardımcılar ─────────────────────────────────────
const getFullUrl = (url) => {
  if (!url) return ''
  if (url.startsWith('http') || url.startsWith('data:')) return url
  return api.defaults.baseURL.replace('/api', '') + url
}

const formatDate = (d) => {
  if (!d) return ''
  return new Date(d).toLocaleDateString('tr-TR', { day:'numeric', month:'long', year:'numeric' })
}

// ─── Markdown ve Kod Renklendirme (Marked + Highlight.js) ───
const renderer = new marked.Renderer();

renderer.code = (token) => {
  const code = token.text || '';
  const language = token.lang || '';
  const validLanguage = hljs.getLanguage(language) ? language : 'plaintext';
  const highlighted = hljs.highlight(code, { language: validLanguage }).value;
  
  return `<pre style="background:#0d0d0d;border:1px solid #333;border-radius:8px;padding:1.2rem;overflow-x:auto;margin:1rem 0;"><code class="hljs" style="font-family:monospace;font-size:0.88rem;color:#d4d4d4;">${highlighted}</code></pre>`;
};

renderer.heading = (token) => {
  const text = token.text;
  const level = token.depth;
  if (level === 1) return `<h1 style="color:#fff;margin:2rem 0 1rem;font-size:1.7rem;">${text}</h1>`;
  if (level === 2) return `<h2 style="color:#ff3b1d;margin:2rem 0 0.75rem;font-size:1.35rem;border-bottom:1px solid #333;padding-bottom:0.4rem;">${text}</h2>`;
  if (level === 3) return `<h3 style="color:#e0e0e0;margin:1.5rem 0 0.5rem;font-size:1.1rem;">${text}</h3>`;
  return `<h${level}>${text}</h${level}>`;
};

renderer.blockquote = (token) => {
  const quote = token.text;
  return `<blockquote style="border-left:3px solid #ff3b1d;padding:0.5rem 1rem;margin:1rem 0;color:#aaa;font-style:italic;">${quote}</blockquote>`;
};

renderer.listitem = (token) => {
  const text = token.text;
  return `<li style="margin:0.3rem 0 0.3rem 1.5rem;color:#ccc;">${text}</li>`;
};

renderer.hr = () => {
  return `<hr style="border:none;border-top:1px solid #333;margin:1.5rem 0;">`;
};

marked.setOptions({
  renderer: renderer,
  breaks: true,
  gfm: true
});

const renderMarkdown = (md) => {
  if (!md) return '';
  return marked.parse(md);
}

const renderedMarkdown = computed(() => renderMarkdown(currentPost.value.content))

// ─── Kitap İçindekiler Oluşturma ─────────────────────
const generateBookToc = () => {
  const content = currentPost.value.content || ''
  const lines = content.split('\n')
  const toc = []
  let pageNum = 0
  lines.forEach(line => {
    line = line.trim()
    if (/^###[ \t]*(.+)$/.test(line)) {
      toc.push({ level:3, title: line.replace(/^###[ \t]*/,'').trim(), pageNum: pageNum || 1 })
    } else if (/^##[ \t]*(.+)$/.test(line)) {
      pageNum++
      toc.push({ level:2, title: line.replace(/^##[ \t]*/,'').trim(), pageNum })
    }
  })
  bookToc.value = toc
}

// İçerik değiştiğinde kitap içindekilerini otomatik güncelle
watch(() => currentPost.value.content, () => {
  if (currentPost.value.postType === 'book') {
    generateBookToc()
  }
})

// ─── CRUD ─────────────────────────────────────────────
const loadPosts = async () => {
  loading.value = true
  try {
    const res = await api.get('/BlogPosts')
    posts.value = res.data
    
    // SEO
    try {
      const seoRes = await api.get('/SeoSettings/page?route=/blog')
      if (seoRes.data) {
        seoData.value = seoRes.data
        pageVisibility.value = seoData.value.isVisible !== false && seoData.value.IsVisible !== false
      }
    } catch (e) {}
    
  } catch { errorMsg.value = 'Yazılar yüklenirken hata.' }
  finally { loading.value = false }
}

const saveVisibility = async () => {
  try {
    if (seoData.value) {
       seoData.value.isVisible = pageVisibility.value;
       await api.post('/SeoSettings', seoData.value);
    } else {
       await api.post('/SeoSettings', { route: '/blog', isVisible: pageVisibility.value });
    }
  } catch (e) {
    console.error('Görünürlük kaydedilemedi', e)
  }
}

const loadCategories = async () => {
  try {
    const res = await api.get('/BlogPosts/categories')
    categories.value = res.data
  } catch { console.error('Kategoriler yüklenirken hata.') }
}

const addCategory = async () => {
  if (!newCategoryName.value) return
  try {
    const res = await api.post('/BlogPosts/categories', { name: newCategoryName.value, nameEn: newCategoryNameEn.value, icon: newCategoryIcon.value })
    categories.value.push(res.data)
    newCategoryName.value = ''
    newCategoryNameEn.value = ''
    newCategoryIcon.value = ''
  } catch (err) {
    alert("Kategori eklenemedi!")
  }
}

const deleteCategory = async (id) => {
  if (!confirm("Bu kategoriyi silmek istediğinize emin misiniz?")) return;
  try {
    await api.delete('/BlogPosts/categories/' + id)
    categories.value = categories.value.filter(c => c.id !== id)
  } catch (err) {
    alert("Kategori silinirken hata oluştu.")
  }
}

const openEditor = (post, type = 'article') => {
  if (post) {
    currentPost.value = { ...post }
    rawTags.value = post.tags ? post.tags.join(', ') : ''
    if (post.postType === 'book') generateBookToc()
  } else {
    currentPost.value = {
      id: 0, title:'', titleEn:'', summary:'', summaryEn:'', content:'', contentEn:'',
      blogCategoryId: 0,
      coverImageUrl:'', icon:'', tags:[], techIcons:[],
      proTip:'', proTipEn:'', postType: type,
      bookColor: type === 'book' ? '#1a1a2e' : null
    }
    rawTags.value = ''
    bookToc.value = []
  }
  errorMsg.value = ''
  isEditing.value = true
}

const closeEditor = () => {
  isEditing.value = false
  currentPost.value = {}
  bookToc.value = []
}

const updateTags = () => {
  currentPost.value.tags = rawTags.value.split(',').map(t => t.trim()).filter(Boolean)
}

const addTechIcon = () => {
  if (!newTechName.value) return
  if (!currentPost.value.techIcons) currentPost.value.techIcons = []
  currentPost.value.techIcons.push(newTechIcon.value ? `${newTechIcon.value}|${newTechName.value}` : newTechName.value)
  newTechName.value = ''
}

const removeTechIcon = (idx) => currentPost.value.techIcons.splice(idx, 1)

const refineWithAI = async () => {
  const hintResult = await swal.fire({
    title: 'Özel Denetim Talimatı',
    html: 'QA (Denetim) yapay zekasının <b>özellikle</b> dikkat etmesini istediğiniz bir şey var mı?<br><span style="font-size:0.8rem;color:#888;">Örn: "Başlıkları ve etiketleri çevirmeyi unutma" veya "Sadece ProTip kısmını kontrol et"</span>',
    input: 'textarea',
    inputPlaceholder: 'Buraya yazabilirsiniz... (İsteğe bağlı)',
    showCancelButton: true,
    confirmButtonText: 'Denetimi Başlat',
    cancelButtonText: 'İptal',
    background: '#1a1d24',
    color: '#fff',
    confirmButtonColor: '#3b82f6',
    cancelButtonColor: '#ef4444'
  });

  if (!hintResult.isConfirmed) return;
  const userHint = hintResult.value || null;

  aiLoading.value = true;
  swal.fire({
    title: 'QA Denetimi Başladı...',
    html: 'Dedektif ve Çevirmen yapay zekalar tartışıyor. Bu işlem normalden daha uzun sürebilir...',
    allowOutsideClick: false,
    didOpen: () => {
      swal.showLoading();
    }
  });

  const cleanAIResponse = (text) => {
    if (!text) return text;
    return text
      .replace(/<text>/gi, '')
      .replace(/<\/text>/gi, '')
      .replace(/^```[a-z]*\n?/gi, '') // Baştaki ``` ve olası dil etiketini temizle (örn: ```markdown)
      .replace(/\n?```$/g, '')        // Sondaki ``` etiketini temizle
      .replace(/^"{3,}|"{3,}$/g, '')
      .trim();
  };

  try {
    if (currentPost.value.title && currentPost.value.titleEn) {
      const res = await translationService.refine(currentPost.value.title, currentPost.value.titleEn, 'English', 'Blog', userHint);
      currentPost.value.titleEn = cleanAIResponse(res?.translatedText) || currentPost.value.titleEn;
    }
    if (currentPost.value.summary && currentPost.value.summaryEn) {
      const res = await translationService.refine(currentPost.value.summary, currentPost.value.summaryEn, 'English', 'Blog', userHint);
      currentPost.value.summaryEn = cleanAIResponse(res?.translatedText) || currentPost.value.summaryEn;
    }
    if (currentPost.value.content && currentPost.value.contentEn) {
      const content = currentPost.value.content;
      const contentEn = currentPost.value.contentEn;
      
      if (currentPost.value.postType === 'book') {
        const trChunks = content.split(/\n(?=## )/g).filter(c => c.trim().length > 0);
        const enChunks = contentEn.split(/\n(?=## )/g).filter(c => c.trim().length > 0);
        
        let refinedParts = [];
        let totalPages = trChunks.length;
        
        for (let i = 0; i < totalPages; i++) {
          swal.update({ html: `Sayfa Denetleniyor... <br><b>İşlenen Sayfa: ${i + 1} / ${totalPages}</b><br><span style="font-size:0.8rem;color:#888;">Eksikler ve hatalar düzeltiliyor.</span>` });
          const trChunk = trChunks[i];
          const enChunk = enChunks[i] || trChunk; // Eğer İngilizce eksikse Türkçesini gönderip tamamen baştan çevirt
          
          try {
            const res = await translationService.refine(trChunk, enChunk, 'English', 'Blog', userHint);
            let refined = cleanAIResponse(res?.translatedText) || enChunk;
            
            if (trChunk.trim().startsWith('## ') && !refined.trim().startsWith('## ')) {
               refined = '## ' + refined.replace(/^#+\s*/, '').trimLeft();
            }
            refinedParts.push(refined);
          } catch (err) {
            console.error('Refine error:', err);
            refinedParts.push(enChunk);
          }
          await new Promise(resolve => setTimeout(resolve, 2500));
        }
        currentPost.value.contentEn = refinedParts.join('\n\n');
      } else {
        swal.update({ html: `İçerik Denetleniyor... <br><span style="font-size:0.8rem;color:#888;">Eksikler ve hatalar düzeltiliyor.</span>` });
        const res = await translationService.refine(content, contentEn, 'English', 'Blog', userHint);
        currentPost.value.contentEn = cleanAIResponse(res?.translatedText) || currentPost.value.contentEn;
      }
    }

    swal.fire({
      icon: 'success',
      title: 'Mükemmel!',
      text: 'Çeviriler dedektif onayından geçti ve onarıldı!',
      confirmButtonText: 'Tamam'
    });
  } catch (error) {
    swal.fire({
      icon: 'error',
      title: 'Hata!',
      text: 'Denetim sırasında bir hata oluştu: ' + (error.response?.data?.message || error.message),
    });
  } finally {
    aiLoading.value = false;
  }
};

const translateWithAI = async () => {
  aiLoading.value = true;
  swal.fire({
    title: 'Yapay Zeka Çeviriyor...',
    html: 'Bu işlem içeriğin uzunluğuna bağlı olarak birkaç dakika sürebilir. Lütfen bekleyin...',
    allowOutsideClick: false,
    didOpen: () => {
      swal.showLoading();
    }
  });

  const cleanAIResponse = (text) => {
    if (!text) return text;
    return text
      .replace(/<text>/gi, '')
      .replace(/<\/text>/gi, '')
      .replace(/^```[a-z]*\n?/gi, '') // Baştaki ``` ve olası dil etiketini temizle
      .replace(/\n?```$/g, '')        // Sondaki ``` etiketini temizle
      .replace(/^"{3,}|"{3,}$/g, '') // Bastaki ve sondaki 3 veya daha fazla tırnağı temizler
      .replace(/\(Önyüz\)/gi, '(Frontend)')
      .replace(/\(Arka Plan\)/gi, '(Backend)')
      .replace(/\(Arka Katman\)/gi, '(Backend)')
      .replace(/\(Ara Katman\)/gi, '(Middleware)')
      .trim();
  };

  try {
    if (currentPost.value.title) {
      const res = await translationService.translate(currentPost.value.title, 'English', 'Blog');
      currentPost.value.titleEn = cleanAIResponse(res?.translatedText) || currentPost.value.titleEn;
    }
    if (currentPost.value.summary) {
      const res = await translationService.translate(currentPost.value.summary, 'English', 'Blog');
      currentPost.value.summaryEn = cleanAIResponse(res?.translatedText) || currentPost.value.summaryEn;
    }
    if (currentPost.value.content && !currentPost.value.contentEn) {
      const content = currentPost.value.content;
      const CHUNK_SIZE = 5000;

      if (currentPost.value.postType === 'book') {
        // Kitaplar için Sayfa Sayfa (## ile) ayırma stratejisi
        let translatedParts = [];
        // Kitap sayfalarını "## " başlıklarına göre ayır
        const pageChunks = content.split(/\n(?=## )/g).filter(c => c.trim().length > 0);
        
        let currentPageIndex = 1;
        let totalPages = pageChunks.length;
        
        for (const chunk of pageChunks) {
          swal.update({ html: `Kitap Sayfaları Çevriliyor... Lütfen bekleyin. <br><br><b>İşlenen Sayfa: ${currentPageIndex} / ${totalPages}</b><br><span style="font-size:0.8rem;color:#888;">Yapay zeka sınırlarına takılmamak için yavaş ve güvenli çevriliyor.</span>` });
          
          let finalTranslated = chunk;
          let retryCount = 0;
          let success = false;
          
          while(retryCount < 3 && !success) {
            try {
              const res = await translationService.translate(chunk, 'English', 'Blog');
              let translated = res?.translatedText || chunk;
              finalTranslated = cleanAIResponse(translated);
              
              // GARANTİ SAYFA KORUMASI: Eğer orijinal parça "## " ile başlıyorsa, çeviri de KESİNLİKLE "## " ile başlamalı!
              if (chunk.trim().startsWith('## ')) {
                 if (!finalTranslated.trim().startsWith('## ')) {
                    finalTranslated = finalTranslated.replace(/^#+\s*/, ''); // Eğer # varsa temizle
                    finalTranslated = '## ' + finalTranslated.trimLeft();
                 }
              }
              success = true;
            } catch (err) {
              retryCount++;
              console.error(`Chunk translation error (Attempt ${retryCount}):`, err);
              if (retryCount >= 3) {
                 finalTranslated = chunk; // 3 denemede de başarısız olursa Türkçe kalsın
              } else {
                 swal.update({ html: `Rate limit veya hata oluştu. 5 saniye bekleniyor... <br><b>Sayfa: ${currentPageIndex} / ${totalPages}</b>` });
                 await new Promise(resolve => setTimeout(resolve, 5000));
                 swal.update({ html: `Yeniden deneniyor (Deneme ${retryCount + 1})... <br><b>Sayfa: ${currentPageIndex} / ${totalPages}</b>` });
              }
            }
          }
          
          translatedParts.push(finalTranslated);
          
          currentPageIndex++;
          // Rate limit'e (429 Too Many Requests) takılmamak için 2.5 saniye bekle
          if (currentPageIndex <= totalPages) {
            await new Promise(resolve => setTimeout(resolve, 2500));
          }
        }
        
        currentPost.value.contentEn = translatedParts.join('\n\n');
      } else if (content.length > CHUNK_SIZE) {
        let translatedParts = [];
        
        // Mükemmel Markdown Parçalayıcı (Kod bloklarını ve paragrafları ASLA bölmez)
        const chunks = [];
        let currentChunk = '';
        let inCodeBlock = false;
        
        const lines = content.split('\n');
        
        for (let i = 0; i < lines.length; i++) {
            const line = lines[i];
            
            // Kod bloğu kontrolü (``` ile başlayan satırlar)
            if (line.trim().startsWith('```')) {
                inCodeBlock = !inCodeBlock;
            }
            
            // Bölme koşulu: Kod bloğu içinde değilsek, limit aşıldıysa ve satır boşsa (paragraf sonuysa)
            if (!inCodeBlock && currentChunk.length > CHUNK_SIZE && line.trim() === '') {
                chunks.push(currentChunk);
                currentChunk = '';
                continue; // Boş satırı yutuyoruz ki yeni parça boşlukla başlamasın
            }
            
            // Eğer yazar hiç boş satır bırakmamışsa ve parça ÇOK büyümüşse (zorunlu bölme)
            if (!inCodeBlock && currentChunk.length > CHUNK_SIZE + 1500) {
                chunks.push(currentChunk);
                currentChunk = '';
            }
            
            currentChunk += (currentChunk === '' ? '' : '\n') + line;
        }
        
        if (currentChunk.trim().length > 0) {
            chunks.push(currentChunk);
        }

        let currentChunkIndex = 1;
        let estimatedChunks = chunks.length;

        for (const chunk of chunks) {
          swal.update({ html: `Uzun yazı içeriği çevriliyor... Lütfen bekleyin. <br><br><b>İşlenen Bölüm: ${currentChunkIndex} / ${estimatedChunks}</b><br><span style="font-size:0.8rem;color:#888;">Yapay zeka sınırlarına takılmamak için yavaş ve güvenli çevriliyor.</span>` });
          
          try {
            const res = await translationService.translate(chunk, 'English', 'Blog');
            let translated = res?.translatedText || chunk;
            translatedParts.push(cleanAIResponse(translated));
          } catch (err) {
            console.error("Chunk translation error:", err);
            translatedParts.push(chunk); // Hata olursa orijinalini koy
          }
          
          currentChunkIndex++;
          if (currentChunkIndex <= estimatedChunks) {
            await new Promise(resolve => setTimeout(resolve, 2500));
          }
        }
        
        currentPost.value.contentEn = translatedParts.join('\n\n');
      } else {
        const res = await translationService.translate(content, 'English', 'Blog');
        currentPost.value.contentEn = cleanAIResponse(res?.translatedText || currentPost.value.contentEn);
      }
    }
    if (currentPost.value.proTip) {
      const res = await translationService.translate(currentPost.value.proTip, 'English', 'Blog');
      currentPost.value.proTipEn = cleanAIResponse(res?.translatedText) || currentPost.value.proTipEn;
    }
    
    if (currentPost.value.tags && currentPost.value.tags.length > 0) {
      for (let i = 0; i < currentPost.value.tags.length; i++) {
        const tag = currentPost.value.tags[i];
        if (!tag.includes('|')) {
          const res = await translationService.translate(tag, 'English', 'Blog');
          if (res?.translatedText) {
            currentPost.value.tags[i] = tag + '|' + cleanAIResponse(res.translatedText);
          }
        }
      }
      rawTags.value = currentPost.value.tags.join(', ');
    }
    
    // Kategorileri çevirme
    for (let cat of categories.value) {
      if (cat.name && !cat.nameEn) {
        const res = await translationService.translate(cat.name, 'English', 'Blog');
        cat.nameEn = cleanAIResponse(res?.translatedText) || cat.nameEn;
        await api.put(`/BlogPosts/categories/${cat.id}`, cat); // Update category directly
      }
    }
    
    if (newCategoryName.value && !newCategoryNameEn.value) {
        const res = await translationService.translate(newCategoryName.value, 'English', 'Blog');
        newCategoryNameEn.value = cleanAIResponse(res?.translatedText) || newCategoryNameEn.value;
    }

    swal.fire('Başarılı', 'Metinler ve uzun kitap içeriği başarıyla İngilizceye çevrildi!', 'success');
  } catch (err) {
    swal.fire('Hata', 'Çeviri sırasında bir hata oluştu.', 'error');
  } finally {
    aiLoading.value = false;
  }
}

const clearTranslations = async () => {
  const confirm = await swal.fire({
    title: 'Çevirileri Temizle?',
    text: 'Bu formdaki (Başlık, Özet, İçerik vb.) tüm İngilizce çeviriler silinecektir. Emin misiniz?',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Evet, Temizle',
    cancelButtonText: 'İptal'
  });

  if (confirm.isConfirmed) {
    currentPost.value.titleEn = '';
    currentPost.value.summaryEn = '';
    currentPost.value.contentEn = '';
    currentPost.value.proTipEn = '';
    
    if (currentPost.value.tags && currentPost.value.tags.length > 0) {
      currentPost.value.tags = currentPost.value.tags.map(t => t.includes('|') ? t.split('|')[0] : t);
      rawTags.value = currentPost.value.tags.join(', ');
    }
    
    swal.fire('Temizlendi', 'İngilizce alanlar boşaltıldı. Kaydet butonuna basarak değişiklikleri kaydedebilirsiniz.', 'info');
  }
}

const savePost = async () => {
  if (!currentPost.value.title) { errorMsg.value = 'Başlık zorunludur.'; return }
  
  if (currentPost.value.blogCategoryId === 0) {
    currentPost.value.blogCategoryId = null;
  }

  saving.value = true
  errorMsg.value = ''
  try {
    if (currentPost.value.id > 0) {
      await api.put(`/BlogPosts/${currentPost.value.id}`, currentPost.value)
      successMsg.value = 'Güncellendi!'
    } else {
      await api.post('/BlogPosts', currentPost.value)
      successMsg.value = 'Eklendi!'
    }
    setTimeout(() => { successMsg.value = '' }, 3000)
    isEditing.value = false
    await loadPosts()
  } catch (err) {
    errorMsg.value = 'Kayıt hatası: ' + (err.response?.data || err.message)
  } finally { saving.value = false }
}

const deletePost = async (id) => {
  const r = await swal.fire({ title:'Emin misiniz?', text:'Bu içerik Çöp Kutusuna taşınacak.', icon:'warning', showCancelButton:true, confirmButtonText:'Çöpe At', cancelButtonText:'İptal' })
  if (!r.isConfirmed) return
  try {
    await api.delete(`/BlogPosts/${id}`)
    posts.value = posts.value.filter(p => p.id !== id)
    successMsg.value = 'Çöp kutusuna taşındı.'
    setTimeout(() => { successMsg.value = '' }, 3000)
  } catch (err) { errorMsg.value = 'Silinemedi: ' + (err.response?.data || err.message) }
}

const openTrash = async () => {
  showTrashModal.value = true
  await loadTrash()
}

const loadTrash = async () => {
  loadingTrash.value = true
  try {
    const res = await api.get('/BlogPosts/trash')
    trashItems.value = res.data
  } catch (error) {
    console.error("Çöp kutusu yüklenemedi", error)
  } finally {
    loadingTrash.value = false
  }
}

const restorePost = async (id) => {
  try {
    await api.post(`/BlogPosts/${id}/restore`)
    successMsg.value = 'Başarıyla kurtarıldı.'
    await loadTrash()
    await loadPosts()
    if(trashItems.value.length === 0) showTrashModal.value = false
    setTimeout(() => { successMsg.value = '' }, 3000)
  } catch (error) {
    alert("Kurtarma işlemi başarısız.")
  }
}

const hardDeletePost = async (id) => {
  const confirm = await swal.fire({
    title: 'Kalıcı Olarak Sil?',
    text: 'Bu işlem geri alınamaz!',
    icon: 'error',
    showCancelButton: true,
    confirmButtonText: 'Evet, Tamamen Sil',
    cancelButtonText: 'İptal'
  })

  if (confirm.isConfirmed) {
    try {
      await api.delete(`/BlogPosts/${id}/hard`)
      successMsg.value = 'Kalıcı olarak silindi.'
      await loadTrash()
      if(trashItems.value.length === 0) showTrashModal.value = false
      setTimeout(() => { successMsg.value = '' }, 3000)
    } catch (error) {
      alert("Kalıcı silme işlemi başarısız.")
    }
  }
}

onMounted(() => {
  loadPosts()
  loadCategories()
})
</script>

<style scoped>
.card-thumb {
  height: 160px;
  background: var(--admin-surface-hover);
  border-bottom: 1px solid var(--admin-border);
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  overflow: hidden;
}
.card-thumb .svg-icon-wrapper :deep(svg) {
  width: 3.5rem;
  height: 3.5rem;
  fill: currentColor;
}
.card-thumb-book {
  background: linear-gradient(135deg, #1a1a2e, #16213e);
}
.type-badge {
  position: absolute;
  top: 10px;
  right: 10px;
  padding: 3px 10px;
  border-radius: 20px;
  font-size: 0.72rem;
  font-weight: 700;
}
.badge-book {
  background: rgba(255,59,29,0.2);
  color: #ff3b1d;
  border: 1px solid rgba(255,59,29,0.4);
  position: static;
}
.badge-article {
  background: rgba(59,130,246,0.2);
  color: #60a5fa;
  border: 1px solid rgba(59,130,246,0.3);
  position: static;
}
.admin-card .badge-book,
.admin-card .badge-article { position: absolute; }
.alert-error {
  background: rgba(239,68,68,0.1);
  border: 1px solid var(--admin-danger);
  color: var(--admin-danger);
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 1.5rem;
}
.alert-success {
  background: rgba(16,185,129,0.1);
  border: 1px solid var(--admin-success);
  color: var(--admin-success);
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 1.5rem;
}
.article-preview :deep(h1) { color: #fff; margin: 1.5rem 0 0.75rem; }
.article-preview :deep(h2) { color: #ff3b1d; margin: 1.5rem 0 0.5rem; }
.article-preview :deep(h3) { color: #e0e0e0; margin: 1rem 0 0.4rem; }
.article-preview :deep(p)  { color: #ccc; line-height: 1.8; margin: 0.5rem 0; }
.article-preview :deep(code) { background:rgba(255,59,29,0.1);color:#ff3b1d;padding:2px 6px;border-radius:4px;font-family:monospace; }
.article-preview :deep(pre) { background:#0d0d0d;border:1px solid #333;border-radius:8px;padding:1rem;overflow-x:auto; }
.article-preview :deep(blockquote) { border-left:3px solid #ff3b1d;padding:0.5rem 1rem;color:#aaa;font-style:italic; }
.article-preview :deep(li) { color:#ccc;margin:0.3rem 0 0.3rem 1.5rem; }
.article-preview :deep(.token-comment) { color: #6a9955; font-style: italic; }
.article-preview :deep(.token-string) { color: #ce9178; }
.article-preview :deep(.token-number) { color: #b5cea8; }
.article-preview :deep(.token-keyword) { color: #569cd6; font-weight: bold; }
.article-preview :deep(.token-builtin) { color: #4ec9b0; }
.article-preview :deep(.token-function) { color: #dcdcaa; }
.article-preview :deep(.token-tag) { color: #569cd6; }
.article-preview :deep(.token-attr) { color: #9cdcfe; }
.article-preview :deep(.token-selector) { color: #dcdcaa; }
.article-preview :deep(.token-property) { color: #9cdcfe; }
.article-preview :deep(.token-value) { color: #ce9178; }
</style>
